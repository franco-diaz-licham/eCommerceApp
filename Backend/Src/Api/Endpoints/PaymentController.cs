namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public PaymentController(IPaymentService paymentService, IMapper mapper)
    {
        _paymentService = paymentService;
        _mapper = mapper;
    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult<BasketResponse>> CreateOrUpdatePaymentIntent(int id)
    {
        var result = await _paymentService.CreateOrUpdatePaymentIntent(id);
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
