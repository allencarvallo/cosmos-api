using CosmosApi.Data;
using CosmosApi.DTOs;
using CosmosApi.Models;
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
                    .Select(i => new InvoiceResponse(
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

            app.MapPut("/invoices/{invoiceId}", async Task<IResult> (long invoiceId, CreateInvoiceRequest req, AppDbContext db) =>
            {
                var invoice = await db.Invoices
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice is null)
                {
                    return TypedResults.NotFound();
                }

                invoice.InvoiceDate = req.InvoiceDate;
                invoice.InvoiceAmount = req.InvoiceAmount;
                invoice.CustomerId = req.CustomerId;
                invoice.InvoiceItems = req.InvoiceItems
                    .Select(i => new InvoiceItem
                    {
                        Description = i.Description,
                        Quantity = i.Quantity,
                        Rate = i.Rate,
                        Amount = i.Amount,
                    }).ToList();

                var saved = await db.SaveChangesAsync();
                return saved > 0 ? TypedResults.Ok(invoice) : TypedResults.BadRequest();
            });

            app.MapDelete("/invoices/{invoiceId}", async Task<IResult> (long invoiceId, AppDbContext db) =>
            {
                var invoice = await db.Invoices
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice is null)
                {
                    return TypedResults.NotFound();
                }

                db.Invoices.Remove(invoice);
                var saved = await db.SaveChangesAsync();
                return saved > 0 ? TypedResults.Ok(true) : TypedResults.BadRequest();
            });
        }
    }
}
