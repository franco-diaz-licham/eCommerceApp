using Backend.Src.Infrastructure.Persistence;

namespace Backend.Src.Application.Services;

public class PaymentService(DataContext db, IPaymentGateway payments, IMapper mapper, ILogger<PaymentService> logger) : IPaymentService
{
    private readonly DataContext _db = db;
    private readonly IPaymentGateway _payments = payments;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<Result<BasketDto>> CreateOrUpdatePaymentIntent(int basketId)
    {
        // Get basket
        var basket = await _db.Baskets.Where(x => x.Id == basketId).FirstOrDefaultAsync();
        if (basket is null) return Result<BasketDto>.Fail("Basket not found.", ResultTypeEnum.NotFound);

        if (basket.Coupon != null)
        {
            var discount = await _payments.CalculateDiscountFromAmount(basket.Coupon.RemoteId!, basket.Subtotal);
            basket.SetDiscount(discount);
        }

        var info = await _payments.CreateOrUpdateAsync(basket.TotalToMinorUnits(), "aud", basket.PaymentIntentId);

        if(basket.PaymentIntentId != info.IntentId || basket.ClientSecret != info.ClientSecret)
        {
            basket.AttachPaymentIntentInfo(info.IntentId, info.ClientSecret!);
            var saved = await _db.SaveChangesAsync() > 0;
            if (!saved) return Result<BasketDto>.Fail("Basket could not be updated...", ResultTypeEnum.Invalid);
        }

        var basketDto = _mapper.Map<BasketDto>(basket);
        return Result<BasketDto>.Success(basketDto, ResultTypeEnum.Accepted);
    }

    public async Task<Result<bool>> RemotePaymentWebhook(string jsonBody, string signature)
    {
        var webhookResult = _payments.ParseWebhook(jsonBody, signature);
        if (!webhookResult.Success || webhookResult.Status is null || string.IsNullOrWhiteSpace(webhookResult.IntentId))
        {
            _logger.LogError("Stripe webhook error: {Error}", webhookResult.Error);
            return Result<bool>.Fail("Stripe webhook error.", ResultTypeEnum.Unprocessable);
        }
        if (webhookResult.Status == "succeeded") await HandlePaymentIntentSucceded(webhookResult.IntentId!, webhookResult.AmountReceived!.Value / 100m);
        else await HandlePaymentIntentFailed(webhookResult.IntentId!);
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
