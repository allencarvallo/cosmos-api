using CosmosApi.Data;
using CosmosApi.Documents;
using CosmosApi.DTOs;
using CosmosApi.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Data;

namespace CosmosApi.Endpoints
{
    public static class InvoiceEndpoints
    {
        public static void RegisterInvoiceEndpoints(this WebApplication app)
        {
            app.MapGet("/invoices", async Task<IResult> (AppDbContext db) =>
            {
                var invoices = await db.Invoices
                    .OrderByDescending(x => x.InvoiceId)
                    .Select(i => new InvoiceListResponse(
                        i.InvoiceId,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.InvoiceAmount,
                        i.Customer.Name
                    )).ToListAsync();

                return TypedResults.Ok(invoices);
            });

            app.MapGet("/invoices/{invoiceId}", async Task<IResult> (long invoiceId, AppDbContext db) =>
            {
                var invoice = await db.Invoices
                    .Where(i => i.InvoiceId == invoiceId)
                    .Select(i => new InvoiceResponse(
                        i.InvoiceId,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.InvoiceAmount,
                        i.CustomerId,
                        i.InvoiceItems.Select(item => new InvoiceItemResponse(
                            item.InvoiceItemId,
                            item.Description,
                            item.Quantity,
                            item.Rate,
                            item.Amount)).ToList()))
                    .FirstOrDefaultAsync();

                return invoice is null ? TypedResults.NotFound() : TypedResults.Ok(invoice);
            });

            app.MapPost("/invoices", async Task<IResult> (CreateInvoiceRequest req, AppDbContext db) =>
            {
                using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                try
                {
                    var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    var year = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist).Year;

                    var tracker = await db.InvoiceSequenceTrackers
                        .FirstOrDefaultAsync(t => t.Year == year);

                    if (tracker == null)
                    {
                        tracker = new InvoiceSequenceTracker { Year = year, LastSequence = 0 };
                        db.InvoiceSequenceTrackers.Add(tracker);
                    }

                    tracker.LastSequence += 1;
                    await db.SaveChangesAsync();

                    var invoice = new Invoice
                    {
                        InvoiceNumber = $"COS-{year}-{tracker.LastSequence:D4}",
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
                    await db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return TypedResults.Ok(true);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return TypedResults.BadRequest(false);
                }
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
                return saved > 0 ? TypedResults.Ok(true) : TypedResults.BadRequest();
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

            app.MapGet("/invoices/{invoiceId}/pdf", async Task<IResult> (long invoiceId, AppDbContext db) =>
            {
                var invoice = await db.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice is null)
                    return TypedResults.NotFound();

                var model = new InvoicePdfModel
                {
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    InvoiceAmount = invoice.InvoiceAmount,
                    CustomerName = invoice.Customer.Name,
                    CustomerEmail = invoice.Customer.Email,
                    CustomerPhone = invoice.Customer.Phone,
                    Items = invoice.InvoiceItems.Select(i => new InvoiceItemPdfModel
                    {
                        Description = i.Description,
                        Quantity = i.Quantity,
                        Rate = i.Rate,
                        Amount = i.Amount
                    }).ToList()
                };

                var pdf = new InvoiceDocument(model).GeneratePdf();

                return Results.File(pdf, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
            });
        }
    }
}
