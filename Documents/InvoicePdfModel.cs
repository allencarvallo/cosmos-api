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
        public string CustomerAddress { get; set; } = string.Empty;
        public string PlaceOfSupply { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Cosmos Tailors";
        public string BankAccountNumber { get; set; } = "259847150062";
        public string BankIfscCode { get; set; } = "INDB0000091";
        public string BankName { get; set; } = "INDUSIND BANK";
        public string BankBranch { get; set; } = "KAKKANAD";
        public string LogoPath { get; set; } = "Assets/logo.png";
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
