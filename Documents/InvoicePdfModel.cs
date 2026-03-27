namespace CosmosApi.Documents
{
    public class InvoicePdfModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateOnly InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public List<InvoiceItemPdfModel> Items { get; set; } = [];
    }

    public class InvoiceItemPdfModel
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
