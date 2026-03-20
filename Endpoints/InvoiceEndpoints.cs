using CosmosApi.Data;
using CosmosApi.DTOs;
using CosmosApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CosmosApi.Endpoints
{
    public static class InvoiceEndpoints
    {
        public static void RegisterInvoiceEndpoints(this WebApplication app)
        {
            app.MapGet("/invoices", async Task<IResult> (AppDbContext db) =>
            {
                var invoices = await db.Invoices
                    .Include(i => i.Customer)
                    .Select(i => new InvoiceDto(
                            i.InvoiceId,
                            i.InvoiceNumber,
                            i.InvoiceDate,
                            i.InvoiceAmount,
                            i.Customer.Name
                    )).ToListAsync();

                return TypedResults.Ok(invoices);
            });

            app.MapPost("/invoices", async Task<IResult> (CreateInvoiceRequest req, AppDbContext db) =>
            {
                var invoice = new Invoice
                {
                    InvoiceNumber = Guid.NewGuid().ToString(),
                    InvoiceDate = req.InvoiceDate,
                    InvoiceAmount = req.InvoiceAmount,
                    CustomerId = req.CustomerId,
                    InvoiceItems = req.InvoiceItems
                        .Select(i => new InvoiceItem
                        {
                            Description = i.Description,
                            Quantity = i.Quantity,
                            Rate = i.Rate,
                            Amount = i.Amount,
                        }).ToList()
                };

                db.Invoices.Add(invoice);
                var saved = await db.SaveChangesAsync();

                return saved > 0 ? TypedResults.Ok(true) : TypedResults.BadRequest(false);
            });
        }
    }
}
