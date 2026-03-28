using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CosmosApi.Documents;

public class InvoiceDocument : IDocument
{
    private readonly InvoicePdfModel _model;

    private static readonly string AccentColor = "#cc0000";
    private static readonly string BorderGray = "#000000";
    private static readonly string TextMuted = "#64748b";
    private static readonly string LightGray = "#f5f5f5";

    public InvoiceDocument(InvoicePdfModel model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Invoice {_model.InvoiceNumber}",
        Author = "Cosmos Tailors",
        CreationDate = DateTime.Now
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

            page.Content().Element(ComposeContent);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(0);

            // title
            col.Item().Element(ComposeTitle);

            // company + invoice info
            col.Item().Element(ComposeCompanyHeader);

            // receiver details
            col.Item().Element(ComposeReceiverDetails);

            // items table
            col.Item().Element(ComposeItemsTable);

            // total in words
            col.Item().Element(ComposeTotalInWords);

            // bank details + signatory
            col.Item().Element(ComposeFooter);
        });
    }

    private void ComposeTitle(IContainer container)
    {
        container
            .PaddingBottom(6)
            .AlignCenter()
            .Text("BILL OF SUPPLY")
            .FontSize(14)
            .Bold();
    }

    private void ComposeCompanyHeader(IContainer container)
    {
        container.Border(1).BorderColor(BorderGray).Row(row =>
        {
            // left — company info
            row.RelativeItem(3).Column(col =>
            {
                // company name row
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(6)
                    .Row(r =>
                    {
                        if (File.Exists(_model.LogoPath))
                        {
                            r.ConstantItem(70)
                                .Height(70)
                                .Image(_model.LogoPath)
                                .FitArea();
                        }
                        else
                        {
                            r.ConstantItem(70)
                                .Height(70)
                                .Border(1)
                                .BorderColor(BorderGray)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text("LOGO")
                                .FontSize(8)
                                .FontColor(TextMuted);
                        }

                        r.RelativeItem()
                            .PaddingLeft(8)
                            .AlignMiddle()
                            .Text("COSMOS TAILORS")
                            .FontSize(16)
                            .Bold()
                            .FontColor(AccentColor);
                    });

                // address row
                void InfoRow(string label, string value)
                {
                    col.Item()
                        .BorderBottom(1)
                        .BorderColor(BorderGray)
                        .Row(r =>
                        {
                            r.ConstantItem(80)
                                .BorderRight(1)
                                .BorderColor(BorderGray)
                                .Padding(4)
                                .Text(label)
                                .FontColor(TextMuted);

                            r.RelativeItem()
                                .Padding(4)
                                .Text(value)
                                .Bold();
                        });
                }

                InfoRow("Business Address", "Cosmos Tailors, Peoples Road, Vaduthala\nKochi-682023, Ernakulam, Kerala, India");
                InfoRow("GSTIN", "32AEGPD3257G1ZY");
                InfoRow("Email", "cosmostailors@gmail.com");
                InfoRow("Mobile", "+91 8089110559");
            });

            // right — invoice number + date
            row.ConstantItem(1).Background(BorderGray);

            row.RelativeItem(1).Column(col =>
            {
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(6)
                    .AlignCenter()
                    .Text("Invoice No")
                    .FontColor(TextMuted);

                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(6)
                    .AlignCenter()
                    .Text(_model.InvoiceNumber)
                    .Bold();

                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(6)
                    .AlignCenter()
                    .Text("Dated")
                    .FontColor(TextMuted);

                col.Item()
                    .Padding(6)
                    .AlignCenter()
                    .Text(_model.InvoiceDate.ToString("d-MMM-yyyy"))
                    .Bold();
            });
        });
    }

    private void ComposeReceiverDetails(IContainer container)
    {
        container
            .BorderLeft(1)
            .BorderRight(1)
            .BorderBottom(1)
            .BorderColor(BorderGray)
            .Column(col =>
            {
                // header
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(4)
                    .Text("Details of Receiver")
                    .Bold()
                    .FontSize(8);

                col.Item().Row(row =>
                {
                    // left — receiver info
                    row.RelativeItem(3).Column(info =>
                    {
                        void ReceiverRow(string label, string value)
                        {
                            info.Item()
                                .BorderBottom(1)
                                .BorderColor(BorderGray)
                                .Row(r =>
                                {
                                    r.ConstantItem(60)
                                        .BorderRight(1)
                                        .BorderColor(BorderGray)
                                        .Padding(4)
                                        .Text(label)
                                        .FontColor(TextMuted);

                                    r.RelativeItem()
                                        .Padding(4)
                                        .Text(value)
                                        .Bold();
                                });
                        }

                        ReceiverRow("Name", _model.CustomerName);
                        ReceiverRow("Address", _model.CustomerAddress);
                        ReceiverRow("Email", _model.CustomerEmail);
                        ReceiverRow("Mobile", _model.CustomerPhone);
                    });

                    // right — place of supply
                    row.ConstantItem(1).Background(BorderGray);

                    row.RelativeItem(1).Column(supply =>
                    {
                        supply.Item()
                            .BorderBottom(1)
                            .BorderColor(BorderGray)
                            .Padding(4)
                            .AlignCenter()
                            .Text("Place Of Supply")
                            .FontColor(TextMuted);

                        supply.Item()
                            .Padding(4)
                            .AlignCenter()
                            .Text(_model.PlaceOfSupply)
                            .Bold();
                    });
                });
            });
    }

    private void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(30);  // sl no
                cols.RelativeColumn(4);   // description
                cols.RelativeColumn(1);   // qty
                cols.RelativeColumn(1.5f);// rate
                cols.RelativeColumn(1.5f);// amount
            });

            table.Header(header =>
            {
                void HeaderCell(string text, bool alignRight = false)
                {
                    var cell = header.Cell()
                        .Border(1)
                        .BorderColor(BorderGray)
                        .Background(LightGray)
                        .Padding(5);

                    var aligned = alignRight ? cell.AlignRight() : cell.AlignCenter();
                    aligned.Text(text).Bold().FontSize(9);
                }

                HeaderCell("SL.NO");
                HeaderCell("Description");
                HeaderCell("Qty", true);
                HeaderCell("Rate", true);
                HeaderCell("Amount", true);
            });

            for (int i = 0; i < _model.Items.Count; i++)
            {
                var item = _model.Items[i];

                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignCenter().Text((i + 1).ToString());
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text(item.Description);
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignRight().Text($"₹{item.Rate:N2}");
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignRight().Text($"₹{item.Amount:N2}");
            }

            // empty rows for spacing like the original
            for (int i = 0; i < 3; i++)
            {
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
                table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
            }

            // total row
            var total = _model.Items.Sum(i => i.Amount);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
            table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
            table.Cell().Border(1).BorderColor(BorderGray).Padding(5).Text("");
            table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignRight().Text("Total").Bold();
            table.Cell().Border(1).BorderColor(BorderGray).Padding(5).AlignRight().Text($"₹{total:N2}").Bold();
        });
    }

    private void ComposeTotalInWords(IContainer container)
    {
        var total = _model.Items.Sum(i => i.Amount);

        container
            .BorderLeft(1)
            .BorderRight(1)
            .BorderBottom(1)
            .BorderColor(BorderGray)
            .Column(col =>
            {
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(BorderGray)
                    .Padding(4)
                    .Text("Total In Words")
                    .Bold();

                col.Item()
                    .Padding(4)
                    .Text($"Indian Rupee {NumberToWords((long)total)} Only")
                    .Bold();
            });
    }

    private void ComposeFooter(IContainer container)
    {
        container
            .BorderLeft(1)
            .BorderRight(1)
            .BorderBottom(1)
            .BorderColor(BorderGray)
            .Row(row =>
            {
                // bank details
                row.RelativeItem(3)
                    .BorderRight(1)
                    .BorderColor(BorderGray)
                    .Padding(8)
                    .Column(col =>
                    {
                        col.Item().Text("Bank Details:").Bold().FontSize(9);
                        col.Item().PaddingTop(4).Text($"Account Number:   {_model.BankAccountNumber}");
                        col.Item().Text($"IFSC Code:              {_model.BankIfscCode}");
                        col.Item().Text($"Name of Bank:        {_model.BankName}");
                        col.Item().Text($"Branch:                    {_model.BankBranch}");
                    });

                // signatory
                row.RelativeItem(1)
                    .Padding(8)
                    .AlignBottom()
                    .AlignRight()
                    .Text($"Authorised Signatory For {_model.CompanyName}")
                    .FontSize(8)
                    .FontColor(TextMuted);
            });
    }

    private static string NumberToWords(long number)
    {
        if (number == 0) return "Zero";

        string[] ones = ["", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                         "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                         "Seventeen", "Eighteen", "Nineteen"];
        string[] tens = ["", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"];

        string words = "";

        if (number / 100000 > 0)
        {
            words += NumberToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }
        if (number / 1000 > 0)
        {
            words += NumberToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }
        if (number / 100 > 0)
        {
            words += NumberToWords(number / 100) + " Hundred ";
            number %= 100;
        }
        if (number > 0)
        {
            if (number < 20)
                words += ones[number];
            else
                words += tens[number / 10] + (number % 10 > 0 ? " " + ones[number % 10] : "");
        }

        return words.Trim();
    }
}