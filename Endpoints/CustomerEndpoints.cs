using CosmosApi.Data;
using CosmosApi.DTOs;
using CosmosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmosApi.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void RegisterCustomerEndpoints(this WebApplication app)
        {
            app.MapGet("/customers", async Task<IResult> (AppDbContext db) =>
            {
                var customers = await db.Customers
                    .OrderByDescending(c => c.CustomerId)
                    .Select(c => new CustomerResponse(
                        c.CustomerId,
                        c.Name,
                        c.Phone,
                        c.Email,
                        c.Address,
                        c.Description))
                    .ToListAsync();
                return TypedResults.Ok(customers);
            });

            app.MapGet("/customers/{customerId}", async Task<IResult> (long customerId, AppDbContext db) =>
            {
                var customer = await db.Customers
                    .Where(c => c.CustomerId == customerId)
                    .Select(c => new CustomerResponse(
                        c.CustomerId,
                        c.Name,
                        c.Phone,
                        c.Email,
                        c.Address,
                        c.Description))
                    .FirstOrDefaultAsync();

                return customer is null ? TypedResults.NotFound() : TypedResults.Ok(customer);
            });

            app.MapPost("/customers", async Task<IResult> (CreateCustomerRequest req, AppDbContext db) =>
            {
                var customer = new Customer
                {
                    Name = req.Name,
                    Phone = req.Phone ?? string.Empty,
                    Email = req.Email ?? string.Empty,
                    Address = req.Address ?? string.Empty,
                    Description = req.Description ?? string.Empty
                };

                db.Customers.Add(customer);
                var saved = await db.SaveChangesAsync();
                return saved > 0 ? TypedResults.Created($"/customers/{customer.CustomerId}", customer) : TypedResults.BadRequest();
            });

            app.MapPut("/customers/{customerId}", async Task<IResult> (long customerId, CreateCustomerRequest req, AppDbContext db) =>
            {
                var customer = await db.Customers.FindAsync(customerId);

                if (customer is null)
                    return TypedResults.NotFound();

                customer.Name = req.Name;
                customer.Phone = req.Phone ?? string.Empty;
                customer.Email = req.Email ?? string.Empty;
                customer.Address = req.Address ?? string.Empty;
                customer.Description = req.Description ?? string.Empty;

                var saved = await db.SaveChangesAsync();
                return saved > 0 ? TypedResults.Ok(customer) : TypedResults.BadRequest();
            });

            app.MapDelete("/customers/{customerId}", async Task<IResult> (long customerId, AppDbContext db) =>
            {
                var customer = await db.Customers.FindAsync(customerId);

                if (customer is null)
                    return TypedResults.NotFound();

                db.Customers.Remove(customer);
                var saved = await db.SaveChangesAsync();
                return saved > 0 ? TypedResults.Ok(true) : TypedResults.BadRequest();
            });
        }
    }
}
