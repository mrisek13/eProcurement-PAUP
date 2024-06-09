using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Web;

namespace eProcurement_PAUP.Reports
{
    public class ReportHelper
    {
        public static void GenerateReportHeader(Document document, string title, Font headerFont)
        {
            // Kreiranje zaglavlja
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;

            // Putanja do slike
            string imagePath = HttpContext.Current.Server.MapPath("~/Content/Images/eProcurement.png");
            // Učitavanje slike
            Image logo = Image.GetInstance(imagePath);

            // Postavljanje širine slike na širinu dokumenta minus margine
            float documentWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            float imageWidth = logo.Width;
            float imageHeight = logo.Height;
            float aspectRatio = imageWidth / imageHeight;

            // Ako je širina slike veća od širine dokumenta, skaliraj je na odgovarajuću širinu
            if (imageWidth > documentWidth)
            {
                imageWidth = documentWidth;
                imageHeight = imageWidth / aspectRatio;
            }

            // Postavljanje veličine slike
            logo.ScaleAbsolute(imageWidth, imageHeight);

            // Kreiranje ćelije za logotip
            PdfPCell logoCell = new PdfPCell(logo);
            // Postavljanje okvira ćelije na NONE (bez okvira)
            logoCell.Border = Rectangle.NO_BORDER;
            // Postavljanje horizontalnog poravnanja logotipa na centar
            logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
            // Dodavanje ćelije s logotipom u tablicu zaglavlja
            headerTable.AddCell(logoCell);

            // Dodavanje datuma, vremena i ostalih informacija
            DateTime now = DateTime.Now;
            PdfPCell cell = new PdfPCell(new Phrase($"Datum: {now.ToString("dd.MM.yyyy")}, \nVrijeme: {now.ToString("HH:mm:ss")}", headerFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            headerTable.AddCell(cell);

            // Dodavanje mjesta
            cell = new PdfPCell(new Phrase("Kučan Marof, Zelena ulica 45", headerFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            headerTable.AddCell(cell);

            // Dodavanje broja telefona
            cell = new PdfPCell(new Phrase("Tel: +385 099/1234-678", headerFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            headerTable.AddCell(cell);

            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            Font titleFont = new Font(bf, 16);

            // Dodavanje naslova izvještaja
            cell = new PdfPCell(new Phrase(title, titleFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(cell);

            // Dodavanje zaglavlja u dokument
            document.Add(headerTable);

            // Dodavanje praznog retka za razmak
            document.Add(new Paragraph(" "));
        }

        public static void GenerateReportFooter(Document document, PdfWriter writer, Font footerFont)
        {
            // Stvaranje tablice za postavljanje podnožja
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            // Stvaranje linije za razdvajanje
            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.BorderColorBottom = BaseColor.BLACK;
            footerTable.AddCell(cell);

            // Stvaranje teksta za footer
            PdfPCell footerCell = new PdfPCell(new Phrase("eProcurement © Maja Risek - PAUP 2024", footerFont));
            footerCell.Border = Rectangle.NO_BORDER;
            footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTable.AddCell(footerCell);

            // Postavljanje pozicije podnožja na dno stranice
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.Bottom, writer.DirectContent);

            // Zatvaranje dokumenta
            document.Close();
        }
    }
}