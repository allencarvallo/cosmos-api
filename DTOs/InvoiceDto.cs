namespace CosmosApi.DTOs
{
    public record InvoiceListResponse(
        long InvoiceId,
        string InvoiceNumber,
        DateOnly InvoiceDate,
        decimal InvoiceAmount,
        string CustomerName);

    public record InvoiceResponse(
        long InvoiceId,
        string InvoiceNumber,
        DateOnly InvoiceDate,
        decimal InvoiceAmount,
        long CustomerId,
        List<InvoiceItemResponse> InvoiceItems);

    public record InvoiceItemResponse(
        long InvoiceItemId,
        string Description,
        int Quantity,
        decimal Rate,
        decimal Amount);

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
