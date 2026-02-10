using Amazon.S3.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Certificate.API.Services;

public class CertificateGenerator
{
    public byte[] Generate(string userName, string quizTitle, int score) 
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container => 
        {
            container.Page(page =>
            {

                // Setting Landscape orientation
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);

                // Decorative outer border
                page.Background()
                    .Border(6)
                    .BorderColor(Colors.Blue.Medium)
                    .Padding(10)
                    .Border(1)
                    .BorderColor(Colors.Blue.Lighten3);

                page.Content().Column(col =>
                {
                    // Using uniform spacing to manage vertical flow without manual PaddingTop
                    col.Spacing(12);

                    // Title Section
                    col.Item().AlignCenter().Column(titleCol =>
                    {
                        titleCol.Item().Text("CERTIFICATE").FontSize(44).ExtraBold().FontColor(Colors.Blue.Medium);
                        titleCol.Item().AlignCenter().Text("OF COMPLETION").FontSize(20).SemiBold().FontColor(Colors.Grey.Darken2).LetterSpacing(0.2f);
                    });

                    // Decorative Star Divider - Simplified to prevent overflow
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingTop(12).LineHorizontal(1).LineColor(Colors.Blue.Lighten4);
                        row.ConstantItem(40).AlignCenter().Text("★").FontSize(18).FontColor(Colors.Blue.Medium);
                        row.RelativeItem().PaddingTop(12).LineHorizontal(1).LineColor(Colors.Blue.Lighten4);
                    });

                    // Recipient Information
                    col.Item().AlignCenter().Text("This is to certify that").FontSize(16).Italic().FontColor(Colors.Grey.Darken1);

                    // User Name with a simple underline
                    col.Item().AlignCenter().Column(nameCol =>
                    {
                        nameCol.Item().Text(userName).FontSize(36).Bold().FontColor(Colors.Blue.Darken3);
                        nameCol.Item().Width(350).LineHorizontal(2).LineColor(Colors.Blue.Darken3);
                    });

                    // Assessment Details
                    col.Item().AlignCenter().Text("has successfully completed the assessment for").FontSize(16);
                    col.Item().AlignCenter().Text(quizTitle).FontSize(24).Bold().FontColor(Colors.Black);

                    // Compact Score Box
                    col.Item().AlignCenter().Width(180).Background(Colors.Blue.Lighten5).Border(1).BorderColor(Colors.Blue.Lighten4).Padding(8).Column(scoreCol =>
                    {
                        scoreCol.Item().AlignCenter().Text("ACHIEVED SCORE").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                        scoreCol.Item().AlignCenter().Text($"{score}%").FontSize(28).ExtraBold().FontColor(Colors.Blue.Medium);
                    });

                    // Footer Section - Removed ExtendVertical() to prevent page breaking
                    col.Item().PaddingTop(15).AlignCenter().Column(footerCol =>
                    {
                        footerCol.Item().AlignCenter().Text(DateTime.Now.ToString("MMMM dd, yyyy")).FontSize(12);
                        footerCol.Item().Width(200).LineHorizontal(1).LineColor(Colors.Black);
                        footerCol.Item().AlignCenter().Text("DATE").FontSize(9).Bold();
                    });
                });
            });
        }).GeneratePdf();
    }
}
