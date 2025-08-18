namespace Backend.Src.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly DataContext _db;
    private readonly IRemotePaymentService _remotePaymentService;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly IConfiguration _config;

    public PaymentService(DataContext db, IRemotePaymentService remotePaymentService, IMapper mapper, ILogger<PaymentService> logger, IConfiguration config)
    {
        _db = db;
        _remotePaymentService = remotePaymentService;
        _mapper = mapper;
        _logger = logger;
        _config = config;
    }

    public async Task<Result<BasketDTO>> CreateOrUpdatePaymentIntent(int basketId)
    {
        // Get basket
        var basket = await _db.Baskets.Where(x => x.Id == basketId).FirstOrDefaultAsync();
        if (basket is null) return Result<BasketDTO>.Fail("Basket not found.", ResultTypeEnum.NotFound);

        // Update basket
        var basketDto = _mapper.Map<BasketDTO>(basket);
        var (intentId, clientSecret) = await _remotePaymentService.CreateOrUpdatePaymentIntent(basketDto);
        basket.AttachPaymentIntent(intentId, clientSecret);

        // Save changes
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<BasketDTO>.Fail("Basket could not be updated...", ResultTypeEnum.Invalid);
        return Result<BasketDTO>.Success(basketDto, ResultTypeEnum.Accepted);
    }

    public async Task<Result<bool>> RemotePaymentWebhook(string jsonBody, string signature)
    {
        // Contruct strip event
        var stripeEvent = new Event();
        try
        {
            var webHookSecret = _config.GetValue<string>("StripeSettings:WhSecret") ?? "";
            stripeEvent = EventUtility.ConstructEvent(jsonBody, signature, webHookSecret);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to construct Stripe event.");
            return Result<bool>.Fail("Stripe webhook failed.", ResultTypeEnum.Invalid);
        }

        // Handle payment 
        if (stripeEvent.Data.Object is not PaymentIntent intent) return Result<bool>.Fail("Stripe webhook error.", ResultTypeEnum.Unprocessable);
        if (intent.Status == "succeeded") await HandlePaymentIntentSucceded(intent.Id, intent.AmountReceived);
        else await HandlePaymentIntentFailed(intent.Id);
        return Result<bool>.Success(ResultTypeEnum.Success);
    }

    private async Task<Result<bool>> HandlePaymentIntentSucceded(string intentId, decimal amount)
    {
        // Get order and validate
        var order = await _db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.PaymentIntentId == intentId);
        if (order is null) return Result<bool>.Fail("Stripe webhook error.", ResultTypeEnum.Unprocessable);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Atomic concurrent handle: Only allow changes on pending orders
        var updated = await _db.Orders
                                .Where(o => o.PaymentIntentId == intentId && o.OrderStatusId == (int)OrderStatusEnum.Pending)
                                .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatusId, (int)OrderStatusEnum.Paid));

        if (updated == 1)
        {
            if (order.Total != amount)
            {
                order.MarkPaymentFailed();
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result<bool>.Fail("Order amount mistach.", ResultTypeEnum.Invalid);
            }
            await _db.Baskets.Where(x => x.PaymentIntentId == intentId).ExecuteDeleteAsync();
            await _db.SaveChangesAsync();
        }

        // Save changes
        await transaction.CommitAsync();
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }

    private async Task<Result<bool>> HandlePaymentIntentFailed(string intentId)
    {
        // Get order and validate
        var order = await _db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.PaymentIntentId == intentId);
        if (order is null) return Result<bool>.Fail("Stripe webhook error.", ResultTypeEnum.Unprocessable);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Atomic concurrent handle: Only allow changes on pending orders
        var updated = await _db.Orders
                               .Where(o => o.PaymentIntentId == intentId && o.OrderStatusId == (int)OrderStatusEnum.Pending)
                               .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatusId, (int)OrderStatusEnum.PaymentFailed));
        if (updated == 1)
        {
            // Handle upading product informatio too
            foreach (var oi in order.OrderItems)
            {
                var affected = await _db.Products
                                        .Where(p => p.Id == oi.ProductId)
                                        .ExecuteUpdateAsync(setters => setters
                                        .SetProperty(p => p.QuantityInStock, p => p.QuantityInStock + oi.Quantity));

                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return Result<bool>.Fail($"Unable to update stock for '{oi.Product?.Name ?? oi.ProductId.ToString()}'.", ResultTypeEnum.Invalid);
                }
            }
            await _db.SaveChangesAsync();
        }

        // Save changes
        await transaction.CommitAsync();
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }
}
