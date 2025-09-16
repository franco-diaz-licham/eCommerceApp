namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(IPaymentService paymentService, IMapper mapper) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<ActionResult<BasketResponse>> CreateOrUpdatePaymentIntent(int basketId)
    {
        var result = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();
        var result = await _paymentService.RemotePaymentWebhook(json, signature);
        return result.ToActionResult();
    }
}
