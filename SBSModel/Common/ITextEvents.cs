using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Net;
using SBSResourceAPI;

namespace SBSModel.Common
{
    public class ITextEvents : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
            PdfPTable tabHead = new PdfPTable(new float[] { 3F });
            tabHead.SpacingAfter = 10F;
            PdfPCell cell1;
            PdfPCell cell2 = new PdfPCell();
            PdfPCell cell3 = new PdfPCell();
            tabHead.TotalWidth = 300F;            

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(AppSetting.SERVER_NAME + "/" + AppSetting.SBSTmpAPI + "/Images/logo-sbsolution.png");
            cell1 = new PdfPCell(logo);

            cell1.Border = 0;
            cell2.Border = 0;
            cell3.Border = 0;

            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
            cell3.HorizontalAlignment = Element.ALIGN_LEFT;

            tabHead.AddCell(cell1);
            tabHead.AddCell(cell2);
            tabHead.AddCell(cell3);
            tabHead.WriteSelectedRows(0, -1, document.Left, document.Top, writer.DirectContent);

            //PdfPTable tabHead1 = new PdfPTable(new float[] { 1F });
            //tabHead1.SpacingAfter = 10F;
            //PdfPCell c1 = new PdfPCell(new Phrase("1"));
            //PdfPCell c2 = new PdfPCell(new Phrase("2"));
            //PdfPCell c3 = new PdfPCell(new Phrase("3"));
            //tabHead1.TotalWidth = 300F;

            //tabHead1.AddCell(c1);
            //tabHead1.AddCell(c2);
            //tabHead1.AddCell(c3);
            //tabHead1.WriteSelectedRows(55, -1, document.Left, document.Top + 50, writer.DirectContent);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(2);
            PdfPCell cell1;
            PdfPCell cell2 = new PdfPCell(new Phrase("Page 1 of 1"));
            tabFot.TotalWidth = document.PageSize.Width - 65;
            cell1 = new PdfPCell(new Phrase(Resource.Message_This_Is_A_Computer_Generated_Invoice));

            cell1.Border = 0;
            cell2.Border = 0;

            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;

            tabFot.AddCell(cell1);
            tabFot.AddCell(cell2);            
            
            tabFot.WriteSelectedRows(0, -1, document.Left, document.Bottom, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}
