using MassTransit.CourierDemo.Shared;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.PaymentService.Activities;

public class PayOrderActivity : IExecuteActivity<PayDto>
{
    private readonly IBus _bus;
    private readonly IRequestClient<IOrderAmountRequest> _client;
    private readonly ILogger<PayOrderActivity> _logger;

    public PayOrderActivity(IBus bus,IRequestClient<IOrderAmountRequest> client,ILogger<PayOrderActivity> logger)
    {
        _bus = bus;
        _client = client;
        _logger = logger;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<PayDto> context)
    {
        var orderRequestClient =
            context.CreateRequestClient<IOrderAmountRequest>(_bus,
                new Uri(RequestAddressConst.OrderAmountRequestAddress));

        var response = await _client.GetResponse<IOrderAmountResponse>(new { context.Arguments.OrderId });
        
        // do payment

        if (response.Message.Amount % 2 == 0)
        {
            _logger.LogInformation($"Order [{context.Arguments.OrderId}] paid successfully!");
            return context.Completed();
        }
        _logger.LogWarning($"Order [{context.Arguments.OrderId}] payment failed!");

        return context.Faulted(new Exception("Order payment failed due to insufficient account balance."));
    }
}