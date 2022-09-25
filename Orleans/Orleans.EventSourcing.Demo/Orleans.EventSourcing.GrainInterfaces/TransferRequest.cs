namespace Orleans.EventSourcing.GrainInterfaces
{
    public class TransferRequest
    {
        public Guid From { get; set; }
        public Guid To { get; set; }

        public Decimal Amount { get; set; }
    }
}
