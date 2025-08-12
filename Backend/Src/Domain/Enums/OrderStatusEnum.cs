namespace Backend.Src.Domain.Enums;

public enum OrderStatusEnum
{
    Pending = 1,
    PaymentReceived = 2,
    PaymentFailed = 3,
    PaymentMismatch = 4
}
