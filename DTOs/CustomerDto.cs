namespace CosmosApi.DTOs
{
    public record CustomerResponse(
        long CustomerId,
        string Name,
        string Phone,
        string Email,
        string Description);

    public record CreateCustomerRequest(
        string Name,
        string? Phone,
        string? Email,
        string? Description);
}
