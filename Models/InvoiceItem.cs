namespace CosmosApi.Models
{
    public class InvoiceItem
    {
        public long InvoiceItemId { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }

        public long InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;
    }
}
