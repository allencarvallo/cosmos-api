namespace CosmosApi.Models
{
    public class Invoice
    {
        public long InvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateOnly InvoiceDate { get; set; }

        public decimal InvoiceAmount { get; set; }

        public long CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = [];
    }
}
