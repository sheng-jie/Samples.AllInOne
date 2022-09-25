using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing.Grains
{
    public class BankAccount
    {
        public Guid BankNo { get; set; }
        public decimal Balance { get; set; }
        
        public BankAccount Apply(InitialAccountEvent initialEvent)
        {
            this.BankNo = initialEvent.BankNo;
            this.Balance = initialEvent.Amount;
            return this;
        }

        public BankAccount Apply(PayEvent payEvent)
        {
            this.Balance -= payEvent.Amount;
            return this;
        }
        
        public BankAccount Apply(TransferInEvent transferEvent)
        {
            this.Balance += transferEvent.Amount;
            return this;
        }
        
        public BankAccount Apply(TransferOutEvent transferEvent)
        {
            this.Balance -= transferEvent.Amount;
            return this;
        }
    }
}
