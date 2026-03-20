namespace CosmosApi.DTOs
{
    public record InvoiceResponse(
        long InvoiceId,
        string InvoiceNumber,
        DateOnly InvoiceDate,
        decimal InvoiceAmount,
        string CustomerName);

    public record CreateInvoiceRequest(
        DateOnly InvoiceDate,
        decimal InvoiceAmount,
        long CustomerId, 
        List<CreateInvoiceItemRequest> InvoiceItems);

    public record CreateInvoiceItemRequest(
        string Description,
        int Quantity,
        decimal Rate,
        decimal Amount);
}
