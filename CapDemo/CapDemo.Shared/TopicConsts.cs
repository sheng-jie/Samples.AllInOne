using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapDemo.Shared
{
    public  class TopicConsts
    {
        public const string CancelOrderCommand = "capdemo.order.cancle";
        public const string DeduceInventoryCommand = "capdemo.inventory.deduce";
        public const string ReturnInventoryTopic = "capdemo.inventory.return";
        public const string PayOrderCommand = "capdemo.payment.pay";
        public const string PayOrderSucceedTopic = "capdemo.payment.pay.succeed";
    }
}
