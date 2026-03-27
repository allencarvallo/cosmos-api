using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CosmosApi.Documents
{
    public class InvoiceDocument : IDocument
    {
        private readonly InvoicePdfModel _model;

        private static readonly string AccentColor = "#1a56db";
        private static readonly string LightGray = "#f8f9fa";
        private static readonly string White = "#ffffff";
        private static readonly string BorderGray = "#e2e8f0";
        private static readonly string TextMuted = "#64748b";

        public InvoiceDocument(InvoicePdfModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Invoice {_model.InvoiceNumber}",
            Author = "Cosmos",
            CreationDate = DateTime.Now
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(20).Row(row =>
            {
                // company name / logo area
                row.RelativeItem().Column(col =>
                {
                    col.Item()
                        .Text("COSMOS TAILORS")
                        .FontSize(26)
                        .Bold()
                        .FontColor(AccentColor);

                    col.Item()
                        .Text("Your trusted business partner")
                        .FontSize(9)
                        .FontColor(TextMuted);
                });

                // invoice title + number
                row.ConstantItem(200).Column(col =>
                {
                    col.Item()
                        .AlignRight()
                        .Text("INVOICE")
                        .FontSize(22)
                        .Bold()
                        .FontColor(TextMuted);

                    col.Item()
                        .AlignRight()
                        .Text($"# {_model.InvoiceNumber}")
                        .FontSize(11)
                        .FontColor(AccentColor)
                        .Bold();
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(16);

                // divider
                col.Item().LineHorizontal(1).LineColor(BorderGray);

                // invoice meta + customer info
                col.Item().Row(row =>
                {
                    // bill to
                    row.RelativeItem().Column(info =>
                    {
                        info.Item()
                            .Text("BILL TO")
                            .FontSize(8)
                            .Bold()
                            .FontColor(TextMuted)
                            .LetterSpacing(0.1f);

                        info.Item().PaddingTop(4).Text(_model.CustomerName).Bold().FontSize(11);

                        if (!string.IsNullOrEmpty(_model.CustomerEmail))
                            info.Item().Text(_model.CustomerEmail).FontColor(TextMuted);

                        if (!string.IsNullOrEmpty(_model.CustomerPhone))
                            info.Item().Text(_model.CustomerPhone).FontColor(TextMuted);
                    });

                    // invoice details
                    row.ConstantItem(200).Column(details =>
                    {
                        details.Item()
                            .Text("INVOICE DETAILS")
                            .FontSize(8)
                            .Bold()
                            .FontColor(TextMuted)
                            .LetterSpacing(0.1f);

                        details.Item().PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().Text("Invoice Date:").FontColor(TextMuted);
                            r.RelativeItem().AlignRight().Text(_model.InvoiceDate.ToString("MMM dd, yyyy")).Bold();
                        });

                        details.Item().PaddingTop(2).Row(r =>
                        {
                            r.RelativeItem().Text("Due Date:").FontColor(TextMuted);
                            r.RelativeItem().AlignRight()
                                .Text(_model.InvoiceDate.AddDays(30).ToString("MMM dd, yyyy"))
                                .Bold();
                        });

                        details.Item().PaddingTop(2).Row(r =>
                        {
                            r.RelativeItem().Text("Status:").FontColor(TextMuted);
                            r.RelativeItem().AlignRight()
                                .Text("UNPAID")
                                .Bold()
                                .FontColor("#e53e3e");
                        });
                    });
                });

                // items table
                col.Item().Element(ComposeItemsTable);

                // totals
                col.Item().Element(ComposeTotals);

                // notes
                col.Item().PaddingTop(8).Column(notes =>
                {
                    notes.Item()
                        .Text("NOTES")
                        .FontSize(8)
                        .Bold()
                        .FontColor(TextMuted)
                        .LetterSpacing(0.1f);

                    notes.Item()
                        .PaddingTop(4)
                        .Text("Thank you for your business. Please make payment within 30 days of the invoice date.")
                        .FontColor(TextMuted)
                        .FontSize(9);
                });
            });
        }

        private void ComposeItemsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4); // description
                    cols.RelativeColumn(1); // qty
                    cols.RelativeColumn(2); // rate
                    cols.RelativeColumn(2); // amount
                });

                // header
                table.Header(header =>
                {
                    void HeaderCell(string text, bool alignRight = false)
                    {
                        var cell = header.Cell()
                            .Background(AccentColor)
                            .Padding(8);

                        var aligned = alignRight ? cell.AlignRight() : cell.AlignLeft();

                        aligned.Text(text)
                            .FontColor(Colors.White)
                            .Bold()
                            .FontSize(9)
                            .LetterSpacing(0.05f);
                    }

                    HeaderCell("DESCRIPTION");
                    HeaderCell("QTY", true);
                    HeaderCell("RATE", true);
                    HeaderCell("AMOUNT", true);
                });

                // rows
                for (int i = 0; i < _model.Items.Count; i++)
                {
                    var item = _model.Items[i];
                    var bg = i % 2 == 0 ? White : LightGray;

                    table.Cell().Background(bg).Padding(8).Text(item.Description);
                    table.Cell().Background(bg).Padding(8).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().Background(bg).Padding(8).AlignRight().Text($"₹{item.Rate:N2}");
                    table.Cell().Background(bg).Padding(8).AlignRight().Text($"₹{item.Amount:N2}");
                }
            });
        }

        private void ComposeTotals(IContainer container)
        {
            var subtotal = _model.Items.Sum(i => i.Amount);
            var tax = subtotal * 0.00m; // adjust tax rate as needed
            var total = subtotal + tax;

            container.AlignRight().Width(240).Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(BorderGray);

                col.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Text("Subtotal").FontColor(TextMuted);
                    row.ConstantItem(100).AlignRight().Text($"₹{subtotal:N2}");
                });

                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Tax (0%)").FontColor(TextMuted);
                    row.ConstantItem(100).AlignRight().Text($"₹{tax:N2}");
                });

                col.Item().PaddingTop(8).LineHorizontal(1).LineColor(BorderGray);

                col.Item()
                    .Background(AccentColor)
                    .Padding(8)
                    .Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL DUE").FontColor(Colors.White).Bold();
                        row.ConstantItem(100).AlignRight().Text($"₹{total:N2}").FontColor(Colors.White).Bold().FontSize(12);
                    });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor(BorderGray).PaddingTop(8).Row(row =>
            {
                row.RelativeItem()
                    .Text("Generated by CosmosApi")
                    .FontSize(8)
                    .FontColor(TextMuted);

                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Page ").FontSize(8).FontColor(TextMuted);
                    x.CurrentPageNumber().FontSize(8).FontColor(TextMuted);
                    x.Span(" of ").FontSize(8).FontColor(TextMuted);
                    x.TotalPages().FontSize(8).FontColor(TextMuted);
                });
            });
        }
    }
}
