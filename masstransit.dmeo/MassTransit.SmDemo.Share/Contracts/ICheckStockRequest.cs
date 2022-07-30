namespace MassTransit.SmDemo.Share.Contracts;

public interface ICheckStockRequest
{
}

public interface ICheckStockResponse
{
    public Dictionary<string, uint> Stocks { get; set; }
}
