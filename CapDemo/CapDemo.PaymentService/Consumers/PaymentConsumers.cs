using CapDemo.Shared;
using CapDemo.Shared.Models;
using DotNetCore.CAP;

namespace CapDemo.PaymentService.Consumers;

public class PaymentConsumers:ICapSubscribe
{
    private readonly ICapPublisher _capPublisher;
    private readonly ILogger<PaymentConsumers> _logger;

    public PaymentConsumers(ICapPublisher capPublisher,ILogger<PaymentConsumers> logger)
    {
        _capPublisher = capPublisher;
        _logger = logger;
    }
    [CapSubscribe(TopicConsts.PayOrderCommand)]
    public async Task<PayResult> Pay(PayDto payDto)
    {
        bool isSucceed = false;
        if (payDto.Amount % 2 == 0)
        {
            isSucceed = true;
            _logger.LogInformation($"Order [{payDto.OrderId}] paid successfully!");
            await _capPublisher.PublishAsync(TopicConsts.PayOrderSucceedTopic, payDto.OrderId);
        }
        else
        {
            isSucceed = false;
            _logger.LogWarning($"Order [{payDto.OrderId}] payment failed!");
        }

        return new PayResult(payDto.OrderId, isSucceed);
    }
}