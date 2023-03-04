namespace CapDemo.Shared.Models;

public class PayDto
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }

    public PayDto(string orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }
}

public class PayResult
{
    public string OrderId { get; set; }
    public bool IsSucceed { get; set; }

    public PayResult(string orderId, bool isSucceed)
    {
        OrderId = orderId;
        IsSucceed = isSucceed;
    }
}