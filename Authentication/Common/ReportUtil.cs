using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Web.Mvc;
using SBSResourceAPI;
using SBSModel.Common;

namespace Authentication.Common
{
    public class ReportUtil
    {


    }
}


public class PDFPageEvent : iTextSharp.text.pdf.PdfPageEventHelper
{
    // This is the contentbyte object of the writer
    PdfContentByte cb;

    // we will put the final number of pages in a template
    PdfTemplate template;

    // this is the BaseFont we are going to use for the header / footer
    BaseFont bf = null;

    // this is the font size page number footer
    // Added by sun 27-06-2016
    float fontsizepagenumber = 8;

    // This keeps track of the creation time
    public DateTime PrintTime { get; set; }
    public byte[] Logoleft { get; set; }
    public byte[] LogoRight { get; set; }
    public string Title { get; set; }
    public string HeaderLeft { get; set; }
    public string HeaderRight { get; set; }
    public Font HeaderFont { get; set; }
    public Font FooterFont { get; set; }
    public string Footer1 { get; set; }
    public string Footer2 { get; set; }
    public byte[] LogoLeftOnFirstPage { get; set; }
    public string CountryName { get; set; }

    // write on top of document
    public override void OnOpenDocument(PdfWriter writer, Document document)
    {
        base.OnOpenDocument(writer, document);
        try
        {
            if (PrintTime == null)
            {
                PrintTime = DateTime.Now;
            }
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            //Added by sun 27-06-2016
            if(System.Web.HttpContext.Current != null)
            {
                HttpCookie langCookie = System.Web.HttpContext.Current.Request.Cookies["culture"];
                if (!string.IsNullOrEmpty(CountryName))
                {
                    if (CountryName == "TH")
                    {
                        var configurationPath = System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf");
                        if (configurationPath != null)
                        {
                            bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                            fontsizepagenumber = 12;
                        }
                    }
                }
                if (langCookie != null)
                {
                    if (langCookie.Value == "th")
                    {
                        var configurationPath = System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf");
                        if (configurationPath != null)
                        {
                            bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                            fontsizepagenumber = 12;
                        }
                    }
                }
            }            
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
            if (LogoLeftOnFirstPage != null)
            {
                Image imgleft = Image.GetInstance(LogoLeftOnFirstPage);

                if (imgleft != null)
                {
                    PdfPTable table = new PdfPTable(1)
                    {
                        TotalWidth = document.PageSize.Width - 80,
                        LockedWidth = true
                    };

                    float hleftpercen = ((35 * 130) / imgleft.PlainHeight);

                    imgleft.ScalePercent(hleftpercen);
                    imgleft.Alignment = Image.LEFT_ALIGN;

                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    cell.AddElement(imgleft);
                    table.AddCell(cell);
                    table.WriteSelectedRows(0, -1, document.PageSize.GetLeft(40), document.PageSize.GetTop(30), writer.DirectContent);
                }
            }
        }
        catch (DocumentException de)
        {
            Console.WriteLine(de.Message);
        }
        catch (System.IO.IOException ioe)
        {
            Console.WriteLine(ioe.Message);
        }
    }

    // write on start of each page
    public override void OnStartPage(PdfWriter writer, Document document)
    {
        base.OnStartPage(writer, document);
        Rectangle pageSize = document.PageSize;

        if (!string.IsNullOrEmpty(Title))
        {
            cb.BeginText();
            cb.SetFontAndSize(bf, 15);
            cb.SetRGBColorFill(50, 50, 200);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
            cb.ShowText(Title);
            cb.EndText();
        }

        if (HeaderLeft + HeaderRight != string.Empty)
        {
            PdfPTable HeaderTable = new PdfPTable(2);
            HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            HeaderTable.TotalWidth = pageSize.Width - 80;
            HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

            PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont));
            HeaderLeftCell.Padding = 5;
            HeaderLeftCell.PaddingBottom = 8;
            HeaderLeftCell.BorderWidthRight = 0;
            HeaderTable.AddCell(HeaderLeftCell);

            PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight, HeaderFont));
            HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            HeaderRightCell.Padding = 5;
            HeaderRightCell.PaddingBottom = 8;
            HeaderRightCell.BorderWidthLeft = 0;
            HeaderTable.AddCell(HeaderRightCell);

            cb.SetRGBColorFill(0, 0, 0);
            HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
        }

        if (Logoleft != null)
        {
            Image imgleft = Image.GetInstance(Logoleft);

            if (imgleft != null)
            {
                if (LogoRight != null)
                {
                    PdfPTable table = new PdfPTable(2)
                    {
                        TotalWidth = pageSize.Width - 80,
                        LockedWidth = true
                    };

                    float hleftpercen = ((30 * 100) / imgleft.PlainHeight);

                    imgleft.ScalePercent(hleftpercen);
                    imgleft.Alignment = Image.LEFT_ALIGN;

                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    cell.AddElement(imgleft);
                    table.AddCell(cell);

                    Image imgRight = Image.GetInstance(LogoRight);

                    float hrightpercen = ((30 * 100) / imgRight.PlainHeight);

                    imgRight.ScalePercent(hrightpercen);
                    imgRight.Alignment = Image.RIGHT_ALIGN;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_MIDDLE;
                    cell.AddElement(imgRight);
                    table.AddCell(cell);
                    document.Add(table);
                }
                else
                {
                    PdfPTable table = new PdfPTable(1)
                    {
                        TotalWidth = pageSize.Width - 80,
                        LockedWidth = true
                    };

                    float hleftpercen = ((30 * 100) / imgleft.PlainHeight);

                    imgleft.ScalePercent(hleftpercen);
                    imgleft.Alignment = Image.LEFT_ALIGN;

                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    cell.AddElement(imgleft);
                    table.AddCell(cell);

                    document.Add(table);
                }
            }
        }
    }

    // write on end of each page
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        base.OnEndPage(writer, document);
        int pageN = writer.PageNumber;
        String text = Resource.Page + " " + pageN + "  " + Resource.Of + "  ";
        float len = bf.GetWidthPoint(text, 8);

        Rectangle pageSize = document.PageSize;


       cb.SetRGBColorFill(100, 100, 100);

        if (Footer1 != null & Footer2 != null)
        {
            float f1len = bf.GetWidthPoint(Footer1, 8);
            float f2len = bf.GetWidthPoint(Footer2, 8);

            float totalwidth = (pageSize.Width) / 2;
            cb.BeginText();
            cb.SetFontAndSize(bf, fontsizepagenumber);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer1, totalwidth - (f1len / 2), pageSize.GetBottom(50), 0);
            cb.EndText();


            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, fontsizepagenumber);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer2, totalwidth - (f2len / 2), pageSize.GetBottom(30), 0);
            cb.EndText();
        }
        else if (Footer1 != null)
        {
            float f1len = bf.GetWidthPoint(Footer1, 8);

            float totalwidth = (pageSize.Width) / 2;
            cb.BeginText();
            cb.SetFontAndSize(bf, fontsizepagenumber);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer1, totalwidth - (f1len / 2), pageSize.GetBottom(30), 0);
            cb.EndText();
        }


        cb.BeginText();
        cb.SetFontAndSize(bf, fontsizepagenumber);
        cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
           text,
            pageSize.GetRight(40),
            pageSize.GetBottom(30), 0);
        cb.EndText();
        cb.AddTemplate(template, pageSize.GetRight(40), pageSize.GetBottom(30));

    }

    //write on close of document
    public override void OnCloseDocument(PdfWriter writer, Document document)
    {
        base.OnCloseDocument(writer, document);
        template.BeginText();
        template.SetFontAndSize(bf, fontsizepagenumber);
        template.SetTextMatrix(0, 0);
        template.ShowText("" + (writer.PageNumber - 1));
        template.EndText();
    }
}