namespace CosmosApi.Models
{
    public class Customer
    {
        public long CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Invoice> Invoices { get; set; } = [];
    }
}
