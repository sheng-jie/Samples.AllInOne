namespace DtmDemo.WebApi.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }

    public class TransferRequest
    {
        public int UserId{ get; set; }
        public decimal Amount { get; set; }

        public TransferRequest(int userId,decimal amount)
        {
            this.UserId = userId;
            this.Amount = amount;
        }
    }
}
