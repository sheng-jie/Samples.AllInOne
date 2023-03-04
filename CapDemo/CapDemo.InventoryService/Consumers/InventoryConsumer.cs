using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CapDemo.Shared;
using CapDemo.Shared.Models;
using DotNetCore.CAP;

namespace CapDemo.InventoryService.Consumers
{
    public class InventoryConsumer : ICapSubscribe
    {
        private readonly ILogger<InventoryConsumer> _logger;
        private readonly ICapPublisher _capPublisher;

        public InventoryConsumer(ILogger<InventoryConsumer> logger, ICapPublisher capPublisher)
        {
            _logger = logger;
            _capPublisher = capPublisher;
        }

        [CapSubscribe(TopicConsts.DeduceInventoryCommand)]
        public async Task DeduceInventory(DeduceInventoryDto deduceStockDto)
        {
            //deduce inventory 

            _logger.LogInformation($"Inventory has been deducted for order [{deduceStockDto.OrderId}]!");
            var amount = deduceStockDto.DeduceStockItems.Sum(t => t.Price * t.Qty);
            await _capPublisher.PublishAsync(TopicConsts.PayOrderCommand, new PayDto(deduceStockDto.OrderId, amount),
                callbackName: TopicConsts.ReturnInventoryTopic);
        }

        [CapSubscribe(TopicConsts.ReturnInventoryTopic)]
        public void ReturnInventory(PayResult payResult)
        {
            //return inventory
            if (!payResult.IsSucceed)
            {
                _logger.LogWarning($"Inventory has been returned for order [{payResult.OrderId}]");
                _capPublisher.PublishAsync(TopicConsts.CancelOrderCommand, payResult.OrderId);
            }
        }
    }
}