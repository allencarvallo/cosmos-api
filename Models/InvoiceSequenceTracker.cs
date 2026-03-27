namespace CosmosApi.Models
{
    public class InvoiceSequenceTracker
    {
        public long InvoiceSequenceTrackerId { get; set; }

        public int Year { get; set; }

        public int LastSequence { get; set; }
    }
}
