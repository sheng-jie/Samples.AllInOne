using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing.GrainInterfaces
{
    public class PayRequest
    {
        public string BillNo { get; set; }
        public decimal Amount { get; set; }
    }
}
