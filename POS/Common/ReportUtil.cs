using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using StarMicronics.StarIO;
using System.Text;
using POS.Models;
using System.Management;
using SBSModel.Common;
using SBSModel.Models;

namespace POS.Common {
    static class Commands
    {
        public const string DEFAULT_NEW_LINE = "\n";
        public const string LINE_BREAK_FULL = "------------------------------------------------\n";

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //     Kick Cash Drawer
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        /// <summary>
        /// Open cash drawer command is 0x07
        /// </summary>
        public const string OPEN_CASH_DRAWER = "\x07";
        //END OPEN DRAWER CMD ########################################################


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //     Font Set
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        /// <summary>
        /// Set the font to Font A
        /// 0x1B 0x1E 0x46 0x00
        /// This is the BEST font to use with Star Printers.
        /// </summary>
        public const string SELECT_FONT_A = "\x1B\x1E\x46\x00";

        /// <summary>
        /// Set the font to Font B
        /// 0x1B 0x1E 0x46 0x00
        /// This is the BEST font to use with Star Printers.
        /// </summary>
        public const string SELECT_FONT_B = "\x1B\x1E\x46\x01";

        /// <summary>
        /// Set the font to Font OCR B
        /// 0x1B 0x1E 0x46 0x10
        /// This may not work with all star printers, use with caution.
        /// </summary>
        public const string SELECT_FONT_OCRB = "\x1b\x1e\x46\x10";
        //END FONT CMDS ##############################################################

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //     Feed Commmands
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        /// <summary>
        /// Line Feed
        /// 0x0A
        /// </summary>
        public const string LINE_FEED = "\x0a";

        /// <summary>
        /// Command to set the printer for 3mm feeds
        /// 0x1B 0x7A 0x00
        /// </summary>
        public const string LINE_FEED_3mm = "\x1b\x7a\x00";

        /// <summary>
        /// Command to set the printer for 4mm feeds
        /// 0x1B 0x7A 0x01
        /// </summary>
        public const string LINE_FEED_4mm = "\x1b\x7a\x01";

        /// <summary>
        /// Form Feed
        /// 0x0C
        /// </summary>
        public const string FORM_FEED = "\x0c";

        /// <summary>
        /// TEXT ALIGNMENT
        /// Command for text alignment.
        /// ESC  GS a n
        /// n = 0,1,2
        /// </summary>
        public const string ALIGN_LEFT = "\x1b\x1d\x61\x0";
        public const string ALIGN_CENTER = "\x1b\x1d\x61\x1";
        public const string ALIGN_RIGHT = "\x1b\x1d\x61\x2";

        /// <summary>
        /// Initializes Horizontal Tab Value
        /// </summary>
        public const string INIT_H_TAB = "\x1b\x44\x2\x10\x22\x0";
        /// <summary>
        /// Horizontal Tab
        /// </summary>
        public const string H_TAB = "\x9";

        /// <summary>
        /// IMPORTANT! Messages to the printer will not print without this specific command.
        /// </summary>
        public const string PARTIAL_CUT = "\x1b\x64\x01";
        public const string FULL_CUT = "\x1b\x64\x00";
        public const string FEED_FULL_CUT = "\x1b\x64\x02";

        /// <summary>
        /// Emphasize/Bold Print Start
        /// </summary>
        public const string BOLD_PRNT_START = "\x1b\x45";
        /// <summary>
        /// Emphasize/Bold Print End
        /// </summary>
        public const string BOLD_PRNT_END = "\x1b\x46";

        /// <summary>
        /// Start Black & White Invert
        /// </summary>
        public const string BW_INVERT_START = "\x1b\x34";
        /// <summary>
        /// END Black & White Invert
        /// </summary>
        public const string BW_INVERT_END = "\x1b\x35";

        /// <summary>
        /// START Underline
        /// </summary>
        public const string UNDERLINE_START = "\x1b\x2d\x1";
        /// <summary>
        /// END Underline
        /// </summary>
        public const string UNDERLINE_END = "\x1b\x2d\x0";


        /// <summary>
        /// LEFT MARGIN
        /// Command for setting a left margin (in dots).
        /// 0x1B 0x6C n
        /// The thrid byte can increment for the margin size.
        /// </summary>
        public static string SetMarginLeft(int val)
        {
            return "\x1b\x6c" + val.ToString();
        }
        /// <summary>
        /// RIGHT MARGIN
        /// Command for setting a left margin (in dots).
        /// 0x1B 0x51 n
        /// The thrid byte can increment for the margin size.
        /// </summary>
        public static string SetMarginRight(int val)
        {
            return "\x1b\x51" + val.ToString();
        }

        /// <summary>
        /// Expand the character
        /// </summary>
        /// <param name="height">Height of character</param>
        /// <param name="width">Width of character</param>
        /// <returns></returns>
        public static string SetCharacterResize(int height, int width)
        {
            return "\x1b\x69" + String.Format("{0:X}", height) + String.Format("{0:X}", width);
        }
        public const string CANCEL_CharacterResize = "\x1b\x69\x0\x0";

        public static string PadLeft(string keyword, int padCount = 0)
        {
            if (padCount == 0)
            {
                return keyword.PadLeft(14, ' ');
            }
            else
            {
                return keyword.PadLeft(padCount, ' ');
            }
        }
    }

    static class WebPRNTCommands
    {
        public enum Alignment
        {
            Left,
            Right,
            Center
        }
        public const string DEFAULT_NEW_LINE = "nline;";
        public const string LINE_BREAK_FULL = "<text>------------------------------------------------nline;</text>";
        public const string LINE_FEED1 = @"<feed line=""1""/>";
        public const string LINE_FEED2 = @"<feed line=""2""/>";
        public const string LINE_FEED3 = @"<feed line=""3""/>";
        public const string INITIALIZE = "<initialization/>";
        public const string START_TEXT_ELEMENT = "<text>";
        public const string END_TEXT_ELEMENT = "</text>";
        public const string BOLD_PRNT_START = @"<text emphasis=""true""/>";
        public const string BOLD_PRNT_END = @"<text emphasis=""false""/>";
        //public const string CUT_PAPER_FEED = @"<cutpaper feed=""true""/>";
        public static string PadLeft(string keyword, int padCount = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(keyword);
            sb.Insert(0, "\x20", padCount);

            return sb.ToString();
        }
        public static string SetCharacterSize(int height, int width, bool emphasize = false)
        {
            if (emphasize)
            {
                return @"<text width=""" + width.ToString() + @""", height=""" + height.ToString() + @""", emphasis=""true""/>";
            }
            else
            {
                return @"<text width=""" + width.ToString() + @""", height=""" + height.ToString() + @""", emphasis=""false""/>";
            }
        }
        public static string SetCharacterSpace(int space)
        {
            return @"<text characterspace=""" + space.ToString() + @"""/>";
        }
        public static string SetAlignment(Alignment val)
        {
            return @"<alignment position=""" + val.ToString().ToLower() + @"""/>";
        }
        public static string SetText(string val)
        {
            return val.Replace(" ", "\x20").Replace(System.Environment.NewLine, "nline;");
        }
        public static string CutPaper(bool feed = false)
        {
            return @"<cutpaper feed=""" + feed.ToString().ToLower() + @"""/>";
        }

    }

    public class ReportUtil
    {

        public static string RightAlginAddSpace(string left, string right)
        {
            var str = "";
            int maxlinechar = 48;
            int maxchar = 36;
            bool havePrintRight = false;

            left = left.Replace("\t", "");

            var leftfoo = Regex.Split(left, "\n");
            for (int linecnt = 0; linecnt < leftfoo.Length; linecnt++)
            {

                string line = leftfoo[linecnt];

                if (leftfoo.Length > 1 && linecnt != 0)
                    line = "\t" + line;

                if (line.Length > maxchar)
                {
                    string tempStr = "";
                    var foo = Regex.Split(line, " ");
                    for (int spacecnt = 0; spacecnt < foo.Length; spacecnt++)
                    {

                        string word = foo[spacecnt];
                        if ((tempStr + " " + word).Length <= maxchar)
                        {

                            if (spacecnt == 0)
                                tempStr = tempStr + word;
                            else
                                tempStr = tempStr + " " + word;

                            if (spacecnt == foo.Length - 1)
                                str = str + tempStr + "\r\n";
                        }
                        else
                        {
                            if (havePrintRight == false)
                            {
                                int space = (maxlinechar - (tempStr.Length + right.Length));
                                string padstring = "".PadRight(space);
                                tempStr = tempStr + padstring + right;
                                havePrintRight = true;
                            }
                            if (spacecnt == foo.Length - 1)
                            {
                                str = str + tempStr + "\r\n\t" + word + "\r\n";
                                if (linecnt < leftfoo.Length - 1)
                                    str = "\t" + str;
                            }
                            else
                            {
                                str = str + tempStr + "\r\n\t";
                                tempStr = word;
                            }
                        }
                    }
                }
                else
                {
                    if (havePrintRight == false)
                    {

                        int space = (maxlinechar - (line.Length + right.Length));
                        string padstring = "".PadRight(space);
                        line = line + padstring + right;
                        havePrintRight = true;

                    }
                    str = str + line + "\r\n";
                }
            }
            return str;
        }

        public static string RightAlginAddSpace(string left, string mid, string right)
        {
            var str = "";
            int maxlinechar = 47;
            int maxchar = 22;
            bool havePrintRight = false;

            left = left.Replace("\t", "");

            var leftfoo = Regex.Split(left, "\n");
            for (int linecnt = 0; linecnt < leftfoo.Length; linecnt++)
            {

                string line = leftfoo[linecnt];

                if (leftfoo.Length > 1 && linecnt != 0)
                    line = "\t" + line;

                if (line.Length > maxchar)
                {
                    string tempStr = "";
                    var foo = Regex.Split(line, " ");
                    for (int spacecnt = 0; spacecnt < foo.Length; spacecnt++)
                    {

                        string word = foo[spacecnt];
                        if ((tempStr + " " + word).Length <= maxchar)
                        {

                            if (spacecnt == 0)
                                tempStr = tempStr + word;
                            else
                                tempStr = tempStr + " " + word;

                            if (spacecnt == foo.Length - 1)
                                str = str + tempStr + "\r\n";
                        }
                        else
                        {
                            if (havePrintRight == false)
                            {
                                tempStr = tempStr + mid.PadLeft((maxchar - tempStr.Length) + 5, ' ');
                                int space = (maxlinechar - (tempStr.Length + right.Length));
                                string padstring = "".PadRight(space);
                                tempStr = tempStr + padstring + right;
                                havePrintRight = true;
                            }
                            if (spacecnt == foo.Length - 1)
                            {
                                str = str + tempStr + "\r\n\t" + word + "\r\n";
                                if (linecnt < leftfoo.Length - 1)
                                    str = "\t" + str;
                            }
                            else
                            {
                                str = str + tempStr + "\r\n\t";
                                tempStr = word;
                            }
                        }
                    }
                }
                else
                {
                    if (havePrintRight == false)
                    {
                        int space = maxchar - line.Length;
                        string padstring = "".PadRight(space);
                        line = line + padstring + mid.PadLeft(5, ' ');
                        space = (maxlinechar - (line.Length + right.Length));
                        padstring = "".PadRight(space);
                        line = line + padstring + right;
                        havePrintRight = true;

                    }
                    str = str + line + "\r\n";
                }
            }
            return str;
        }

        public static string printToPrinter(string prnData, string ipAddress)
        {
            var msg = "";
            if (string.IsNullOrEmpty(ipAddress))
            {
                try
                {
                    var scope = new ManagementScope(@"\root\cimv2");
                    if (!scope.IsConnected)
                        scope.Connect();

                    var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                    var results = searcher.Get();
                    foreach (var printer in results)
                    {
                        if (!string.IsNullOrEmpty(ipAddress))
                            break;

                        var portName = printer.Properties["PortName"].Value;

                        var searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_TCPIPPrinterPort where Name LIKE '" + portName + "'");
                        var results2 = searcher2.Get();
                        try
                        {
                            foreach (var printer2 in results2)
                            {
                                ipAddress = printer2.Properties["HostAddress"].Value.ToString();
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                    msg = "Can't connect receipt printer.";
                    return msg; //exit this entire function
                }
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                msg = "Can't connect receipt printer.";
                return msg; //exit this entire function
            }

            IPort sPort = null;
            //Set the status to offline because this is a new attempt to print
            var onlineStatus = false;

            //TRY -> Use the textboxes to check if the port info given will connect to the printer.
            try
            {
                //Very important to only try opening the port in a Try, Catch incase the port is not working

                sPort = Factory.I.GetPort("TCP:" + ipAddress, "", 10000);



                //GetOnlineStatus() will return a boolean to let us know if the printer was reachable.
                onlineStatus = sPort.GetOnlineStatus();
            }

            //CATCH -> If the port information is bad, catch the failure.
            catch (PortException)
            {
                if (sPort != null)
                    Factory.I.ReleasePort(sPort);
                msg = "Can't get port receipt printer.";
                return msg; //exit this entire function
            }

            //If it is offline, dont setup receipt or try to write to the port.
            if (onlineStatus == false)
            {
                Factory.I.ReleasePort(sPort);
            }
            //Else statement means it is ONLINE, lets start the printjob
            else
            {
                byte[] dataByteArray = Encoding.UTF8.GetBytes(prnData);

                //Write bytes to printer
                uint amountWritten = 0;
                uint amountWrittenKeep;
                while (dataByteArray.Length > amountWritten)
                {
                    //This while loop is very important because in here is where the bytes are being sent out through StarIO to the printer
                    amountWrittenKeep = amountWritten;
                    try
                    {
                        amountWritten += sPort.WritePort(dataByteArray, amountWritten, (uint)dataByteArray.Length - amountWritten);
                    }
                    catch (PortException)
                    {
                        // error happen while sending data
                        //this.lblPrinterStatus.Text = "Port Error";
                        //this.lblPrinterStatus.ForeColor = Color.Red;
                        msg = "Can't connect receipt printer.";
                        Factory.I.ReleasePort(sPort);
                        return msg;
                    }

                    if (amountWrittenKeep == amountWritten) // no data is sent
                    {
                        Factory.I.ReleasePort(sPort);
                        //lblPrinterStatus.Text = "Can't send data";
                        //this.lblPrinterStatus.ForeColor = Color.Red;
                        msg = "Can't send data to receipt printer.";
                        return msg; //exit this entire function
                    }
                }

                //Release the port 2
                //THIS IS VERY IMPORTANT, IF YOU OPEN THE PORT AND DO NOT CLOSE IT, YOU WILL HAVE PROBLEMS
                Factory.I.ReleasePort(sPort);
                //////////////////////////

                //Send the data to the log, you can take this out of the code in your application

            }
            return msg;
        }

        public static string receiptData(POS_Receipt rcp, Company_Details company, POS_Receipt_Configuration rcp_config, POS_Terminal terminal, User_Profile cashier, Tax tax)
        {
            var nDatas = new StringBuilder();
            //Added by NayThway on 30-May-2015
            var cbService = new ComboService();

            decimal gstPercentage = 0;
            bool useGST = false;


            decimal totalPaidAmount = 0;
            decimal creditsUsed = 0;
            bool isCreditsRedeemed = false;

            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;

            if (tax != null)
            {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                {
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;
                }
            }

            nDatas.Append("\x1b\x1d\x61\x01\n"); // Alignment (center)
            if (company != null)
            {
                //Edit By sun 18-08-2015
                var country = "";
                if (company.Country1 != null)
                {
                    country = company.Country1.Description;
                }
                nDatas.Append(company.Name + "\r\n");

                nDatas.Append(company.Address + " " + country + " " + company.Zip_Code + "\r\n");
                nDatas.Append("Tel: " + company.Phone + "\r\n");
            }
            if (rcp_config != null)
            {
                nDatas.Append(rcp_config.Receipt_Header + "\r\n\r\n");
            }

            nDatas.Append("\x1b\x1d\x61\x00\x1b\x44\x02\x10\x22\x00");
            nDatas.Append("Receipt No: " + rcp.Receipt_No + "\r\n");

            if (company.Business_Type == BusinessType.FoodAndBeverage)
            {
                if (rcp.Table_No == null) rcp.Table_No = "";
                nDatas.Append("Table No: " + rcp.Table_No.ToString() + "\r\n");
            }

            nDatas.Append("Cashier Name: " + cashier.Name + "\r\n");
            if (terminal != null) nDatas.Append("Terminal: " + terminal.Terminal_Name + "\r\n");

            nDatas.Append(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(rcp.Receipt_Date), "Time: " + DateUtil.ToDisplayTime(rcp.Receipt_Date)));

            if (rcp.Status == ReceiptStatus.Void)
            {

                nDatas.Append(Commands.ALIGN_CENTER); //Center Align
                nDatas.Append(Commands.SetCharacterResize(1, 1));
                nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
                nDatas.Append(Commands.DEFAULT_NEW_LINE + "VOID" + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END
                nDatas.Append(Commands.CANCEL_CharacterResize);

            }
            else if (rcp.Status == ReceiptStatus.Hold)
            {

                nDatas.Append(Commands.ALIGN_CENTER); //Center Align
                nDatas.Append(Commands.SetCharacterResize(1, 1));
                nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
                nDatas.Append(Commands.DEFAULT_NEW_LINE + "HOLD BILL" + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END
                nDatas.Append(Commands.CANCEL_CharacterResize);

            }

            nDatas.Append("------------------------------------------------\r\n");

            //-------------------------- receipt product --------------------------


            if (rcp.POS_Products_Rcp != null && rcp.POS_Products_Rcp.Count() > 0)
            {
                var i = 1;
                foreach (POS_Products_Rcp p in rcp.POS_Products_Rcp.ToList())
                {
                    var productname = p.Product_Name;

                    var product = i.ToString() + ". " + productname + " X" + (p.Qty.HasValue ? p.Qty.Value : 0);
                    var totalprice = 0M;
                    if (p.Qty.HasValue && p.Qty.Value > 0 && p.Price.HasValue && p.Price.Value > 0)
                    {
                        totalprice = p.Qty.Value * p.Price.Value;
                    }
                    nDatas.Append(ReportUtil.RightAlginAddSpace(product, totalprice.ToString("n2")));
                    i++;
                }
            }


            nDatas.Append("------------------------------------------------\r\n");

            if (rcp.Status == ReceiptStatus.Void)
            {

                nDatas.Append(Commands.ALIGN_CENTER); //Center Align
                nDatas.Append(Commands.SetCharacterResize(1, 1));
                nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
                nDatas.Append(Commands.DEFAULT_NEW_LINE + "VOID" + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END
                nDatas.Append(Commands.CANCEL_CharacterResize);

            }
            else if (rcp.Status == ReceiptStatus.Hold)
            {

                nDatas.Append(Commands.ALIGN_CENTER); //Center Align
                nDatas.Append(Commands.SetCharacterResize(1, 1));
                nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
                nDatas.Append(Commands.DEFAULT_NEW_LINE + "HOLD BILL" + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END
                nDatas.Append(Commands.CANCEL_CharacterResize);

            }

            string strSubtotalText = "";

            if (rcp.Status != ReceiptStatus.Hold)
                strSubtotalText = "Subtotal (Qty ";
            else
                strSubtotalText = "Amt. due (Qty ";

            nDatas.Append(ReportUtil.RightAlginAddSpace(strSubtotalText + (rcp.Total_Qty.HasValue ? rcp.Total_Qty.Value : 0).ToString("n0") + ")", (rcp.Total_Amount.HasValue ? rcp.Total_Amount.Value : 0).ToString("n2")));

            if (serviceChargeInclude)
            {
                
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Svc. Charge (" + rcp.Service_Charge_Rate + "%)", (rcp.Service_Charge.HasValue ? rcp.Service_Charge.Value : 0).ToString("n2")));
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Total", ((rcp.Total_Amount.HasValue ? rcp.Total_Amount.Value : 0) + (rcp.Service_Charge.HasValue ? rcp.Service_Charge.Value : 0)).ToString("n2")));
                
            }

            if (rcp.Status != ReceiptStatus.Hold)
            {
                nDatas.Append(ReportUtil.RightAlginAddSpace("Discount", (rcp.Total_Discount.HasValue ? rcp.Total_Discount.Value : 0).ToString("n2")));

                if (rcp.Member != null) 
                {
                    decimal memberDiscount = 0;

                    if (rcp.Member_Discount_Type == "%") {
                        memberDiscount = rcp.Total_Amount.Value * (rcp.Member_Discount.Value / 100);
                    } else {
                        memberDiscount = rcp.Member_Discount.Value;
                    }

                    nDatas.Append(ReportUtil.RightAlginAddSpace("Member Discount", (memberDiscount).ToString("n2")));
                }

                //Added by NayThway on 30-May-2015
                if (useGST)
                {
                    nDatas.Append(ReportUtil.RightAlginAddSpace("GST (" + gstPercentage + "% Inclusive)", (rcp.Total_GST_Amount.HasValue ? rcp.Total_GST_Amount.Value : 0).ToString("n2")));
                }

                decimal surchargeAmount = 0;
                string surchargeDisplay = "";
                decimal surchargeRate = 0;

                foreach (var row in rcp.POS_Receipt_Payment)
                {
                    if (row.Payment_Type == PaymentType.Credit_Card)
                    {

                        if (surchargeInclude)
                        {
                            surchargeDisplay = surchargePercen.ToString("n0");
                            surchargeRate = surchargePercen;

                            var s = surchargePercen.ToString().Split('.');
                            if (s.Length > 1)
                            {
                                var precision = s[1];
                                if (NumUtil.ParseDecimal(precision) > 0)
                                {
                                    surchargeDisplay = surchargePercen.ToString("n2");
                                    surchargeRate = surchargePercen;
                                }
                            }

                            surchargeAmount = (decimal)(surchargeAmount + (row.Payment_Amount * (surchargeRate / 100)));
                        }
                    }
                }
                if (surchargeAmount > 0)
                {
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Surcharge (" + surchargeDisplay + "%)", surchargeAmount.ToString("n2")));
                }

                nDatas.Append("\x1b\x45");
                nDatas.Append(ReportUtil.RightAlginAddSpace("Total", (rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0).ToString("n2")));
                nDatas.Append("\x1b\x46");
            }

            nDatas.Append("------------------------------------------------\r\n");

            if (rcp.Status != ReceiptStatus.Hold)
            {
                foreach (var row in rcp.POS_Receipt_Payment)
                {
                    if (row.Payment_Type == PaymentType.Cash)
                    {
                        nDatas.Append(ReportUtil.RightAlginAddSpace("CASH", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2")));
                        nDatas.Append(ReportUtil.RightAlginAddSpace("Change", (rcp.Changes.HasValue ? rcp.Changes.Value : 0).ToString("n2")));
                    }
                    else if (row.Payment_Type == PaymentType.Nets)
                    {
                        nDatas.Append(ReportUtil.RightAlginAddSpace("NETS", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2")));
                    }
                    else if (row.Payment_Type == PaymentType.Credit_Card)
                    {
                        var cardTypeName = "";
                        if (row.Card_Type.HasValue)
                        {
                            var cardType = cbService.GetLookup(row.Card_Type);
                            if (cardType != null)
                            {
                                cardTypeName = cardType.Name;
                            }
                        }

                        if (!string.IsNullOrEmpty(cardTypeName))
                        {
                            nDatas.Append(ReportUtil.RightAlginAddSpace(cardTypeName.ToUpper(), (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2")));
                        }
                        else
                        {
                            nDatas.Append(ReportUtil.RightAlginAddSpace("Credit/Debit", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2")));
                        }
                    } 
                    else if (row.Payment_Type == PaymentType.Redeem)
                    {
                        isCreditsRedeemed = true;
                        creditsUsed = (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0);
                        nDatas.Append(ReportUtil.RightAlginAddSpace("Redeemed Credits", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2")));
                        nDatas.Append(ReportUtil.RightAlginAddSpace("Remaining Credits", (rcp.Member.Credit.HasValue ? rcp.Member.Credit.Value : 0).ToString("n2")));
                    }

                    totalPaidAmount += row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0;
                }

                //nDatas.Append(ReportUtil.RightAlginAddSpace("Total Paid", (rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0).ToString("n2")));
                nDatas.Append("------------------------------------------------\r\n");
                if (rcp.Status == ReceiptStatus.BackOrder)
                {
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Total Paid", totalPaidAmount.ToString("n2")));
                    decimal remainingAmount = 0;
                    if ((rcp.Net_Amount - totalPaidAmount) > 0)
                    {
                        remainingAmount = rcp.Net_Amount.Value - totalPaidAmount;
                    }
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Remaining Amount", remainingAmount.ToString("n2")));
                    nDatas.Append("------------------------------------------------\r\n");
                    nDatas.Append("\x1b\x45");
                    //Modified by Nay on 07-Oct-2015
                    nDatas.Append(ReportUtil.RightAlginAddSpace(ReceiptStatus.BackOrder, ""));
                    nDatas.Append("\x1b\x46");
                    //nDatas.Append(rcp.Remark + "\r\n");
                    nDatas.Append("Customer Name : " + rcp.Customer_Name + "\r\n");
                    nDatas.Append("Contact No    : " + rcp.Contact_No + "\r\n");
                    nDatas.Append("Email         : " + rcp.Customer_Email + "\r\n");
                    nDatas.Append("Remark        : " + rcp.Remark + "\r\n");
                }
                else
                {
                    nDatas.Append(ReportUtil.RightAlginAddSpace("Total Paid", (rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0).ToString("n2")));


                    if (rcp.Member != null) {
                        nDatas.Append("------------------------------------------------\r\n");
                        nDatas.Append("\x1b\x45");
                        //Modified by Nay on 07-Oct-2015
                        nDatas.Append(ReportUtil.RightAlginAddSpace("Member Details", ""));
                        nDatas.Append("\x1b\x46");
                        nDatas.Append("Name       : " + rcp.Member.Member_Name + "\r\n");
                        nDatas.Append("Contact No : " + rcp.Member.Phone_No + "\r\n");
                        nDatas.Append("Email      : " + rcp.Member.Email + "\r\n");
                        nDatas.Append("Remark     : " + rcp.Remark + "\r\n");

                    } else if (!string.IsNullOrEmpty(rcp.Customer_Name) || !string.IsNullOrEmpty(rcp.Contact_No) || !string.IsNullOrEmpty(rcp.Customer_Email)) {
                        
                        nDatas.Append("------------------------------------------------\r\n");
                        nDatas.Append("\x1b\x45");
                        //Modified by Nay on 07-Oct-2015
                        nDatas.Append(ReportUtil.RightAlginAddSpace("Customer Details", ""));
                        nDatas.Append("\x1b\x46");
                        nDatas.Append("Name       : " + rcp.Customer_Name + "\r\n");
                        nDatas.Append("Contact No : " + rcp.Contact_No + "\r\n");
                        nDatas.Append("Email      : " + rcp.Customer_Email + "\r\n");
                        nDatas.Append("Remark     : " + rcp.Remark + "\r\n");

                    }
                }
                nDatas.Append("------------------------------------------------\r\n");
            }


            nDatas.Append("\r\n");
            nDatas.Append("\x1b\x1d\x61\x01");
            nDatas.Append(rcp_config.Receipt_Footer + "\r\n");
            if (company != null)
            {
                if (!string.IsNullOrEmpty(company.Registry))
                {
                    nDatas.Append("Reg No: " + company.Registry + "\r\n");
                }

                if (!string.IsNullOrEmpty(company.GST_Registration))
                {
                    nDatas.Append("GST Reg No: " + company.GST_Registration + "\r\n");
                }
            }

            nDatas.Append("\r\n\r\n\r\n");
            nDatas.Append("\x1b\x64\x02");
            nDatas.Append("\x07");

            //TEMP Comment
            //if (isCreditsRedeemed) 
            //{
            //    nDatas.Append(Commands.ALIGN_CENTER); //Center Align
            //    nDatas.Append(Commands.DEFAULT_NEW_LINE);
            //    nDatas.Append(rcp.Receipt_No + Commands.DEFAULT_NEW_LINE);
            //    nDatas.Append(ReportUtil.RightAlginAddSpace("Redeemed Credits", (creditsUsed).ToString("n2")));
            //    nDatas.Append(Commands.LINE_BREAK_FULL);
            //    nDatas.Append(ReportUtil.RightAlginAddSpace("Remaining Credits", (rcp.Member.Credit.Value).ToString("n2")));
            //    nDatas.Append("\r\n\r\n\r\n");
            //    nDatas.Append("\x1b\x64\x02");
            //    nDatas.Append("\x07");
            //}


            return nDatas.ToString();
        }

        public static string webPrntReceiptData(POS_Receipt rcp, Company_Details company, POS_Receipt_Configuration rcp_config, POS_Terminal terminal, User_Profile cashier, Tax tax )
        {
            var nDatas = new StringBuilder();
            //Added by NayThway on 30-May-2015
            var cbService = new ComboService();

            decimal gstPercentage = 0;
            bool useGST = false;

            decimal totalPaidAmount = 0;

            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;
            if (tax != null)
            {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;
            }

            nDatas.Append(WebPRNTCommands.INITIALIZE);
            nDatas.Append(WebPRNTCommands.LINE_FEED1);
            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));

            if (company != null)
            {
                //Edit By sun 18-08-2015
                var country = "";
                if (company.Country1 != null)
                {
                    country = company.Country1.Description;
                }
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(company.Name));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(company.Address + " " + country + " " + company.Zip_Code));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText("Tel: " + company.Phone));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            if (rcp_config != null)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(rcp_config.Receipt_Header));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText("Receipt No: " + rcp.Receipt_No));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            if (company.Business_Type == BusinessType.FoodAndBeverage)
            {
                if (rcp.Table_No == null) rcp.Table_No = "";
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText("Table No: " + rcp.Table_No.ToString()));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText("Cashier Name: " + cashier.Name));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            if (terminal != null)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText("Terminal: " + terminal.Terminal_Name));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(rcp.Receipt_Date),
                "Time: " + DateUtil.ToDisplayTime(rcp.Receipt_Date))));
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            if (rcp.Status == ReceiptStatus.Void)
            {
                nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));
                nDatas.Append(WebPRNTCommands.SetCharacterSize(2, 2, true));
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.SetText("VOID"));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1));
            }
            else if (rcp.Status == ReceiptStatus.Hold)
            {
                nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));
                nDatas.Append(WebPRNTCommands.SetCharacterSize(2, 2, true));
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.SetText("HOLD BILL"));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1));
            }

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            ////-------------------------- receipt product --------------------------


            if (rcp.POS_Products_Rcp != null && rcp.POS_Products_Rcp.Count() > 0)
            {
                var i = 1;
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                foreach (POS_Products_Rcp p in rcp.POS_Products_Rcp.ToList())
                {
                    var productname = p.Product_Name;

                    var product = i.ToString() + ". " + productname + " X" + (p.Qty.HasValue ? p.Qty.Value : 0);
                    var totalprice = 0M;
                    if (p.Qty.HasValue && p.Qty.Value > 0 && p.Price.HasValue && p.Price.Value > 0)
                    {
                        totalprice = p.Qty.Value * p.Price.Value;
                    }

                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace(product, totalprice.ToString("n2"))));
                    i++;
                }

                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            if (rcp.Status == ReceiptStatus.Void)
            {
                nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));
                nDatas.Append(WebPRNTCommands.SetCharacterSize(2, 2, true));
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.SetText("VOID"));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1));
            }
            else if (rcp.Status == ReceiptStatus.Hold)
            {
                nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));
                nDatas.Append(WebPRNTCommands.SetCharacterSize(2, 2, true));
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.SetText("HOLD BILL"));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1));
            }

            string strSubtotalText = "";

            if (rcp.Status != ReceiptStatus.Hold)
                strSubtotalText = "Subtotal (Qty ";
            else
                strSubtotalText = "Amt. due (Qty ";

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace(strSubtotalText + (rcp.Total_Qty.HasValue ? rcp.Total_Qty.Value : 0).ToString("n0") + ")", (rcp.Total_Amount.HasValue ? rcp.Total_Amount.Value : 0).ToString("n2"))));
            if (surchargeInclude)
            {
                
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Svc. Charge (" + rcp.Service_Charge_Rate + "%)", (rcp.Service_Charge.HasValue ? rcp.Service_Charge.Value : 0).ToString("n2"))));
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Total", ((rcp.Total_Amount.HasValue ? rcp.Total_Amount.Value : 0) + (rcp.Service_Charge.HasValue ? rcp.Service_Charge.Value : 0)).ToString("n2"))));
                
            }
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            if (rcp.Status != ReceiptStatus.Hold)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Discount", (rcp.Total_Discount.HasValue ? rcp.Total_Discount.Value : 0).ToString("n2"))));

                if (rcp.Member != null) {

                    decimal memberDiscount = 0;

                    if (rcp.Member_Discount_Type == "%") {
                        memberDiscount = rcp.Total_Amount.Value * (rcp.Member_Discount.Value / 100);
                    } else {
                        memberDiscount = rcp.Member_Discount.Value;
                    }

                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Member Discount", (memberDiscount).ToString("n2"))));
                }

                //Added by NayThway on 30-May-2015
                if (useGST)
                {
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("GST (" + gstPercentage + "% Inclusive)", (rcp.Total_GST_Amount.HasValue ? rcp.Total_GST_Amount.Value : 0).ToString("n2"))));
                }

                decimal surchargeAmount = 0;
                string surchargeDisplay = "";
                decimal surchargeRate = 0;

                foreach (var row in rcp.POS_Receipt_Payment)
                {
                    if (row.Payment_Type == PaymentType.Credit_Card)
                    {

                        if (surchargeInclude)
                        {
                            surchargeDisplay = surchargePercen.ToString("n0");
                            surchargeRate = surchargePercen;

                            var s = surchargePercen.ToString().Split('.');
                            if (s.Length > 1)
                            {
                                var precision = s[1];
                                if (NumUtil.ParseDecimal(precision) > 0)
                                {
                                    surchargeDisplay = surchargePercen.ToString("n2");
                                    surchargeRate = surchargePercen;
                                }
                            }

                            surchargeAmount = (decimal)(surchargeAmount + (row.Payment_Amount * (surchargeRate / 100)));
                        }
                    }
                }
                if (surchargeAmount > 0)
                {
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Surcharge (" + surchargeDisplay + "%)", surchargeAmount.ToString("n2"))));
                }

                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Total", (rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0).ToString("n2"))));
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));
            }

            if (rcp.Status != ReceiptStatus.Hold)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

                foreach (var row in rcp.POS_Receipt_Payment)
                {
                    if (row.Payment_Type == PaymentType.Cash)
                    {
                        nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("CASH", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2"))));
                        nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Change", (rcp.Changes.HasValue ? rcp.Changes.Value : 0).ToString("n2"))));
                    }
                    else if (row.Payment_Type == PaymentType.Nets)
                    {
                        nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("NETS", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2"))));
                    }
                    else if (row.Payment_Type == PaymentType.Credit_Card)
                    {
                        var cardTypeName = "";
                        if (row.Card_Type.HasValue)
                        {
                            var cardType = cbService.GetLookup(row.Card_Type);

                            if (cardType != null)
                            {
                                cardTypeName = cardType.Name;
                            }
                        }

                        if (!string.IsNullOrEmpty(cardTypeName))
                        {
                            nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace(cardTypeName.ToUpper(), (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2"))));
                        }
                        else
                        {
                            nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Credit/Debit", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2"))));
                        }
                    } 
                    else if (row.Payment_Type == PaymentType.Redeem) 
                    {
                        nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Redeemed Credits", (row.Payment_Amount.HasValue ? row.Payment_Amount.Value : 0).ToString("n2"))));
                        nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Remaining Credits", (rcp.Member.Credit.HasValue ? rcp.Member.Credit.Value : 0).ToString("n2"))));
                    }

                    totalPaidAmount += row.Payment_Amount.Value;
                }

                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);


                if (rcp.Status == ReceiptStatus.BackOrder)
                {
                    nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Total Paid", totalPaidAmount.ToString("n2"))));
                    decimal remainingAmount = 0;
                    if ((rcp.Net_Amount - totalPaidAmount) > 0)
                    {
                        remainingAmount = rcp.Net_Amount.Value - totalPaidAmount;
                    }
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Remaining Amount", remainingAmount.ToString("n2"))));
                    nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                    nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

                    nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));

                    nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                    nDatas.Append(WebPRNTCommands.SetText(ReceiptStatus.BackOrder));
                    nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                    nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
                    nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

                    nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                    nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                    nDatas.Append(WebPRNTCommands.SetText("Customer Name : " + rcp.Customer_Name + WebPRNTCommands.DEFAULT_NEW_LINE));
                    nDatas.Append(WebPRNTCommands.SetText("Contact No    : " + rcp.Contact_No + WebPRNTCommands.DEFAULT_NEW_LINE));
                    nDatas.Append(WebPRNTCommands.SetText("Email    : " + rcp.Customer_Email + WebPRNTCommands.DEFAULT_NEW_LINE));
                    nDatas.Append(WebPRNTCommands.SetText("Remark    : " + rcp.Remark + WebPRNTCommands.DEFAULT_NEW_LINE));
                    nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                    nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);
                }
                else
                {
                    nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                    nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Total Paid", (rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0).ToString("n2"))));
                    nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                    nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

                    if (rcp.Member != null) {
                        nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));

                        nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                        nDatas.Append(WebPRNTCommands.SetText("Member Details"));
                        nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                        nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
                        nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

                        nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                        nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                        nDatas.Append(WebPRNTCommands.SetText("Name       : " + rcp.Member.Member_Name + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Contact No : " + rcp.Member.Phone_No + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Email      : " + rcp.Member.Email + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Remark     : " + rcp.Remark + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
                        nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

                    } else if (!string.IsNullOrEmpty(rcp.Customer_Name) || !string.IsNullOrEmpty(rcp.Contact_No) || !string.IsNullOrEmpty(rcp.Customer_Email)) {
                        nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));

                        nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                        nDatas.Append(WebPRNTCommands.SetText("Customer Details"));
                        nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                        nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
                        nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

                        nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                        nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                        nDatas.Append(WebPRNTCommands.SetText("Name       : " + rcp.Customer_Name + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Contact No : " + rcp.Contact_No + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Email      : " + rcp.Customer_Email + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.SetText("Remark     : " + rcp.Remark + WebPRNTCommands.DEFAULT_NEW_LINE));
                        nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                        nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);
                    }
                }
            }

            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.SetText(rcp_config.Receipt_Footer));
            if (company != null)
            {
                if (!string.IsNullOrEmpty(company.Registry))
                {
                    nDatas.Append(WebPRNTCommands.SetText("Reg No: " + company.Registry));
                    nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                }
                if (!string.IsNullOrEmpty(company.GST_Registration))
                {
                    nDatas.Append(WebPRNTCommands.SetText("GST Reg No: " + company.GST_Registration));
                    nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                }
            }
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.CutPaper(true));

            return nDatas.ToString();
        }

        public static string dailySalesData(List<POS_Receipt> receipts, Company_Details company, POS_Receipt_Configuration rcp_config,
            POS_Terminal terminal,Tax tax, bool isClosing, Nullable<DateTime> backDate = null)
        {
            var nDatas = new StringBuilder();

            //Added by NayThway on 16-Jun-2015
            var cbService = new ComboService();
            decimal gstPercentage = 0;
            bool useGST = false;


            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;
            if (tax != null)
            {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;
            }

            nDatas.Append(Commands.ALIGN_CENTER); // Alignment (center)
            if (company != null)
            {
                nDatas.Append(company.Name + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(company.Address + " " + (company.Country == null ? "Singapore" : company.Country.Description) + " " + company.Zip_Code + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(company.Phone + "\r\n");
            }
            if (rcp_config != null)
            {
                nDatas.Append(rcp_config.Receipt_Header + Commands.DEFAULT_NEW_LINE);
                nDatas.Append(Commands.DEFAULT_NEW_LINE);
            }

            nDatas.Append("\x1b\x1d\x61\x00\x1b\x44\x02\x10\x22\x00");
            if (terminal != null) nDatas.Append("Terminal: " + terminal.Terminal_Name + Commands.DEFAULT_NEW_LINE);

            if (backDate != null)
            {
                nDatas.Append(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(backDate), "Time: " + DateUtil.ToDisplayTime(backDate)));
            }
            else
            {
                nDatas.Append(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(DateTime.Today), "Time: " + DateUtil.ToDisplayTime(DateTime.Now)));
            }

            nDatas.Append(Commands.ALIGN_CENTER); //Center Align
            nDatas.Append(Commands.SetCharacterResize(1, 1));
            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(Commands.DEFAULT_NEW_LINE + "DAILY SALES" + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END
            nDatas.Append(Commands.CANCEL_CharacterResize);
            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.LINE_BREAK_FULL);

            string shiftStatusMarker = "";
            //------------ FIXED TOTALIZER ------------
            if (!isClosing)
                shiftStatusMarker = "X";
            else
                shiftStatusMarker = "Z";

            nDatas.Append(Commands.ALIGN_LEFT);
            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(shiftStatusMarker.PadRight(17, ' ') + "FIXED TOTALIZER");
            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            nDatas.Append(("GROSS SALES").PadRight(22, ' '));
            string grossSalesNo = receipts.Sum(x => x.Total_Qty).ToString().PadLeft(5, ' ');
            nDatas.Append(grossSalesNo);

            decimal grossSalesValue;

            if (surchargeInclude)
            {
                grossSalesValue = (decimal)receipts.Where(x => x.Status != "Void").Sum(x => x.Total_Amount + x.Service_Charge);
                nDatas.Append(("$" + grossSalesValue.ToString("n2")).PadLeft(20, ' '));
            }
            else
            {
                grossSalesValue = (decimal)receipts.Where(x => x.Status != "Void").Sum(x => x.Total_Amount);
                nDatas.Append(("$" + grossSalesValue.ToString("n2")).PadLeft(20, ' '));
            }

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("NETT SALES").PadRight(20, ' ') + "No");
            string nettSalesNo = receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_Qty).ToString().PadLeft(5, ' ');
            nDatas.Append(nettSalesNo);

            var nettSalesValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_Amount);
            nDatas.Append(("$" + nettSalesValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("CASH/NETS TOTAL").PadRight(27, ' '));
            var cashTotalValue = (decimal)receipts.Where(x => x.Status == "Paid" && (x.Payment_Type == 1 | x.Payment_Type == 2)).Sum(x => x.Total_Amount);
            nDatas.Append(("$" + cashTotalValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("CREDIT TOTAL").PadRight(27, ' '));
            var ccTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount);
            nDatas.Append(("$" + ccTotalValue.ToString("n2")).PadLeft(20, ' '));

            if (surchargeInclude)
            {
                nDatas.Append(Commands.DEFAULT_NEW_LINE);
                nDatas.Append(("SURCHARGES").PadRight(27, ' '));
                var surchargeTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount * (surchargePercen / 100));
                nDatas.Append(("($" + surchargeTotalValue.ToString("n2") + ")").PadLeft(21, ' '));
            }

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            if (serviceChargeInclude)
            {
                nDatas.Append(("SERVICE CHARGES").PadRight(27, ' '));
                var svcChargeTotalValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Service_Charge);
                nDatas.Append(("$" + svcChargeTotalValue.ToString("n2")).PadLeft(20, ' '));
                nDatas.Append(Commands.DEFAULT_NEW_LINE);
            }

            nDatas.Append(("GST TAXABLE").PadRight(27, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append((gstPercentage + "% GST (Inclusive)").PadRight(27, ' '));
            if (!useGST)
            {
                nDatas.Append(("$0.00").PadLeft(20, ' '));
            }
            else
            {
                var gstTotalValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_GST_Amount);
                nDatas.Append(("$" + gstTotalValue.ToString("n2")).PadLeft(20, ' '));
            }

            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.LINE_BREAK_FULL);

            //------------ TRANSACTION ------------
            nDatas.Append(Commands.ALIGN_LEFT);
            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(shiftStatusMarker.PadRight(17, ' ') + "TRANSACTION");
            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            nDatas.Append(("CASH").PadRight(20, ' ') + "No");
            string cashCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Count().ToString().PadLeft(5, ' ');
            nDatas.Append(cashCount);

            decimal cashValue = 0;
            if (serviceChargeInclude)
            {
                cashValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Sum(x => x.Total_Amount + x.Service_Charge);
            }
            else
            {
                cashValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Sum(x => x.Total_Amount);
            }
            nDatas.Append(("$" + cashValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("NETS").PadRight(20, ' ') + "No");
            string netsCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Count().ToString().PadLeft(5, ' ');
            nDatas.Append(netsCount);

            decimal netsValue = 0;
            if (serviceChargeInclude)
            {
                netsValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Sum(x => x.Total_Amount + x.Service_Charge);
            }
            else
            {
                netsValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Sum(x => x.Total_Amount);
            }
            nDatas.Append(("$" + netsValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            ComboService comboServeice = new ComboService();
            Global_Lookup_Data cardTypeLookup;

            int otherCCCount = 0;
            decimal otherCCCardValue = 0;

            foreach (var cType in receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Select(x => x.Card_Type).Distinct().ToList())
            {
                if (cType.HasValue)
                {
                    cardTypeLookup = comboServeice.GetLookup(cType.Value);

                    if (cardTypeLookup != null)
                    {
                        nDatas.Append(cardTypeLookup.Description.ToUpper().PadRight(20, ' ') + "No");
                        string cardCount = receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3 && x.Card_Type == cType)
                            .Count().ToString().PadLeft(5, ' ');
                        nDatas.Append(cardCount);

                        var cardValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3 && x.Card_Type == cType)
                            .Sum(x => x.Payment_Amount);
                        nDatas.Append(("$" + cardValue.ToString("n2")).PadLeft(20, ' '));

                        nDatas.Append(Commands.DEFAULT_NEW_LINE);
                    }
                    else
                    {
                        otherCCCount += receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                            .Where(x => x.Payment_Type == 3 && x.Card_Type == cType).Count();

                        otherCCCardValue += (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                            .Where(x => x.Payment_Type == 3 && x.Card_Type == cType).Sum(x => x.Payment_Amount);
                    }
                }
                else
                {
                    otherCCCount += receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                                .Where(x => x.Payment_Type == 3).Count();

                    otherCCCardValue += (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                        .Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount);
                }

            }

            if (otherCCCount > 0)
            {
                nDatas.Append(("OTHER CC").PadRight(20, ' ') + "No");
                nDatas.Append(otherCCCount.ToString().PadLeft(5, ' '));
                nDatas.Append(("$" + otherCCCardValue.ToString("n2")).PadLeft(20, ' '));
                nDatas.Append(Commands.DEFAULT_NEW_LINE);
            }

            //foreach (var cType in receipts.Where(n => n.Status == "Paid" && n.Payment_Type == 3).Select(x => x.Card_Type).Distinct().ToList()) {

            //    if (cType.HasValue) {
            //        cardTypeLookup = comboServeice.GetLookup(cType.Value);

            //        if (cardTypeLookup != null) {
            //            nDatas.Append(cardTypeLookup.Description.ToUpper().PadRight(20, ' ') + "No");
            //            string cardCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 3 && x.Card_Type == cType)
            //                .Count().ToString().PadLeft(5, ' ');
            //            nDatas.Append(cardCount);

            //            var cardValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 3 && x.Card_Type == cType)
            //                .Sum(x => x.Net_Amount);
            //            nDatas.Append(("$" + cardValue.ToString("n2")).PadLeft(20, ' '));                    
            //        }

            //    } else {
            //        nDatas.Append(("OTHER CC").PadRight(20, ' ') + "No");
            //        string cardCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 3 && x.Card_Type == cType)
            //            .Count().ToString().PadLeft(5, ' ');
            //        nDatas.Append(cardCount);

            //        var cardValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 3 && x.Card_Type == cType)
            //            .Sum(x => x.Net_Amount);
            //        nDatas.Append(("$" + cardValue.ToString("n2")).PadLeft(20, ' '));
            //    }

            //    nDatas.Append(Commands.DEFAULT_NEW_LINE);

            //}


            if (surchargeInclude && surchargePercen > 0)
            {
                nDatas.Append(("SURCHARGES").PadRight(27, ' '));
                var surchargeTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount * (surchargePercen / 100));
                nDatas.Append(("($" + surchargeTotalValue.ToString("n2") + ")").PadLeft(21, ' '));
                nDatas.Append(Commands.DEFAULT_NEW_LINE);
            }

            nDatas.Append(("HOLD BILL").PadRight(20, ' ') + "No");
            string holdCount = receipts.Where(x => x.Status == "Hold").Count().ToString().PadLeft(5, ' ');
            nDatas.Append(holdCount);

            var holdValue = (decimal)receipts.Where(x => x.Status == "Hold").Sum(x => x.Total_Amount);
            nDatas.Append(("$" + holdValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("VOID").PadRight(20, ' ') + "No");
            string voidCount = receipts.Where(x => x.Status == "Void").Count().ToString().PadLeft(5, ' ');
            nDatas.Append(voidCount);

            var voidValue = (decimal)receipts.Where(x => x.Status == "Void").Sum(x => x.Total_Amount);
            nDatas.Append(("$" + voidValue.ToString("n2")).PadLeft(20, ' '));

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("RECEIPT").PadRight(20, ' ') + "No");
            string receiptCount = receipts.Count().ToString().PadLeft(5, ' ');
            nDatas.Append(receiptCount);

            nDatas.Append(Commands.DEFAULT_NEW_LINE);

            nDatas.Append(("DISPLAY TABLE#").PadRight(20, ' ') + "No");
            nDatas.Append(receiptCount);

            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.LINE_BREAK_FULL);

            //------------ CATEGORY ------------
            nDatas.Append(Commands.ALIGN_LEFT);
            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(shiftStatusMarker.PadRight(17, ' ') + "CATEGORY");
            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            int categoryCount = 0;
            decimal categoryValue = 0.00m;
            int categoryTotalCount = 0;
            decimal categoryTotalAmt = 0.00m;

            foreach (var cat in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null).Select(y => y.Product.Product_Category.Category_Name)).Distinct().ToList())
            {
                categoryCount = (int)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                    y => y.Product_ID != null && y.Product.Product_Category.Category_Name == cat)).Sum(z => z.Qty);

                categoryValue = (decimal)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                    y => y.Product_ID != null && y.Product.Product_Category.Category_Name == cat)).Sum(z => z.Price * z.Qty);

                nDatas.Append(RightAlginAddSpace(cat.ToUpper(), categoryCount.ToString(), ("$" + categoryValue.ToString("n2"))));

                categoryTotalCount += categoryCount;
                categoryTotalAmt += categoryValue;
            }

            // OPEN ITEM CATEGORY
            if (receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Count() > 0)
            {

                categoryCount = (int)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Sum(z => z.Qty);
                categoryValue = (decimal)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Sum(z => z.Price * z.Qty);

                nDatas.Append(RightAlginAddSpace("OPEN ITEM", categoryCount.ToString(), ("$" + categoryValue.ToString("n2"))));

                categoryTotalCount += categoryCount;
                categoryTotalAmt += categoryValue;

            }

            nDatas.Append(Commands.LINE_BREAK_FULL);

            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(("TOTAL").PadRight(22, ' '));
            nDatas.Append(categoryTotalCount.ToString().PadLeft(5, ' '));
            nDatas.Append(("$" + categoryTotalAmt.ToString("n2")).PadLeft(20, ' '));
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.LINE_BREAK_FULL);

            //------------ PRODUCT ------------
            nDatas.Append(Commands.ALIGN_LEFT);
            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(shiftStatusMarker.PadRight(17, ' ') + "PRODUCT");
            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            int productCount = 0;
            decimal productValue = 0.00m;
            int productTotalCount = 0;
            decimal productTotalAmt = 0.00m;

            foreach (var cat in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null).Select(y => y.Product.Product_Category.Category_Name)).Distinct().ToList())
            {
                foreach (var prod in receipts.SelectMany(x => x.POS_Products_Rcp.Where(y => y.Product_ID != null && y.Product.Product_Category.Category_Name == cat).Select(y => y.Product.Product_Name)).Distinct().ToList())
                {
                    productCount = (int)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                       y => y.Product_ID != null && y.Product.Product_Name == prod)).Sum(z => z.Qty);

                    productValue = (decimal)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                        y => y.Product_ID != null && y.Product.Product_Name == prod)).Sum(z => z.Price * z.Qty);

                    nDatas.Append(RightAlginAddSpace(prod.ToUpper(), productCount.ToString(), ("$" + productValue.ToString("n2"))));

                    productTotalCount += productCount;
                    productTotalAmt += productValue;
                }
            }

            // OPEN ITEM PRODUCTS

            foreach (var openProduct in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)))
            {
                productCount = (int)openProduct.Qty;
                productValue = (decimal)(openProduct.Price * openProduct.Qty);

                nDatas.Append(RightAlginAddSpace(openProduct.Product_Name, productCount.ToString(), ("$" + productValue.ToString("n2"))));

                productTotalCount += productCount;
                productTotalAmt += productValue;
            }

            nDatas.Append(Commands.LINE_BREAK_FULL);

            nDatas.Append(Commands.BOLD_PRNT_START); //Emphasis/Bold START
            nDatas.Append(("TOTAL").PadRight(22, ' '));
            nDatas.Append(productTotalCount.ToString().PadLeft(5, ' '));
            nDatas.Append(("$" + productTotalAmt.ToString("n2")).PadLeft(20, ' '));
            nDatas.Append(Commands.BOLD_PRNT_END); //Emphasis/Bold END

            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.LINE_BREAK_FULL);

            nDatas.Append(Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE + Commands.DEFAULT_NEW_LINE);
            nDatas.Append(Commands.FEED_FULL_CUT);
            return nDatas.ToString();
        }

        public static string webPrntSalesData(List<POS_Receipt> receipts, Company_Details company, POS_Receipt_Configuration rcp_config,
        POS_Terminal terminal,Tax tax, bool isClosing, Nullable<DateTime> backDate = null)
        {
            var nDatas = new StringBuilder();

            var cbService = new ComboService();
            decimal gstPercentage = 0;
            bool useGST = false;


            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;
            if (tax != null)
            {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;
            }

            nDatas.Append(WebPRNTCommands.INITIALIZE);
            nDatas.Append(WebPRNTCommands.LINE_FEED1);
            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));

            if (company != null)
            {
                //Edit By sun 18-08-2015
                var country = "";
                if (company.Country1 != null)
                {
                    country = company.Country1.Description;
                }
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(company.Name));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(company.Address + " " + country + " " + company.Zip_Code));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText("Tel: " + company.Phone));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            if (rcp_config != null)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText(rcp_config.Receipt_Header));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));

            if (terminal != null)
            {
                nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
                nDatas.Append(WebPRNTCommands.SetText("Terminal: " + terminal.Terminal_Name));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            }

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            if (backDate != null)
            {
                nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(backDate), "Time: " + DateUtil.ToDisplayTime(backDate))));
            }
            else
            {
                nDatas.Append(WebPRNTCommands.SetText(ReportUtil.RightAlginAddSpace("Date: " + DateUtil.ToDisplayDate2(DateTime.Today), "Time: " + DateUtil.ToDisplayTime(DateTime.Today))));
            }
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Center));

            nDatas.Append(WebPRNTCommands.SetCharacterSize(2, 2, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.SetText("DAILY SALES"));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            string shiftStatusMarker = "";
            //------------ FIXED TOTALIZER ------------
            if (!isClosing)
                shiftStatusMarker = "X";
            else
                shiftStatusMarker = "Z";

            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(shiftStatusMarker.PadRight(17, ' ') + "FIXED TOTALIZER"));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE + WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.SetText(("GROSS SALES").PadRight(22, ' ')));
            string grossSalesNo = receipts.Sum(x => x.Total_Qty).ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(grossSalesNo));

            decimal grossSalesValue;

            if (serviceChargeInclude)
            {
                grossSalesValue = (decimal)receipts.Where(x => x.Status != "Void").Sum(x => x.Total_Amount + x.Service_Charge);
                nDatas.Append(WebPRNTCommands.SetText(("$" + grossSalesValue.ToString("n2")).PadLeft(20, ' ')));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            }
            else
            {
                grossSalesValue = (decimal)receipts.Where(x => x.Status != "Void").Sum(x => x.Total_Amount);
                nDatas.Append(WebPRNTCommands.SetText(("$" + grossSalesValue.ToString("n2")).PadLeft(20, ' ')));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            }

            nDatas.Append(WebPRNTCommands.SetText(("NETT SALES").PadRight(20, ' ') + "No"));
            string nettSalesNo = receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_Qty).ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(nettSalesNo));

            var nettSalesValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_Amount);
            nDatas.Append(WebPRNTCommands.SetText(("$" + nettSalesValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("CASH/NETS TOTAL").PadRight(27, ' ')));
            var cashTotalValue = (decimal)receipts.Where(x => x.Status == "Paid" && (x.Payment_Type == 1 | x.Payment_Type == 2)).Sum(x => x.Total_Amount);
            nDatas.Append(WebPRNTCommands.SetText(("$" + cashTotalValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("CREDIT TOTAL").PadRight(27, ' ')));
            var ccTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount);
            nDatas.Append(WebPRNTCommands.SetText(("$" + ccTotalValue.ToString("n2")).PadLeft(20, ' ')));

            if (surchargeInclude)
            {
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                nDatas.Append(WebPRNTCommands.SetText(("SURCHARGES").PadRight(27, ' ')));
                var surchargeTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount * (surchargePercen / 100));
                nDatas.Append(WebPRNTCommands.SetText(("($" + surchargeTotalValue.ToString("n2") + ")").PadLeft(21, ' ')));
            }
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            if (serviceChargeInclude)
            {
                nDatas.Append(WebPRNTCommands.SetText(("SERVICE CHARGES").PadRight(27, ' ')));
                var svcChargeTotalValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Service_Charge);
                nDatas.Append(WebPRNTCommands.SetText(("$" + svcChargeTotalValue.ToString("n2")).PadLeft(20, ' ')));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            }

            nDatas.Append(WebPRNTCommands.SetText(("GST TAXABLE").PadRight(27, ' ')));

            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText((gstPercentage + "% GST (Inclusive)").PadRight(27, ' ')));

            if (!useGST)
            {
                nDatas.Append(WebPRNTCommands.SetText(("$0.00").PadLeft(20, ' ')));
            }
            else
            {
                var gstTotalValue = (decimal)receipts.Where(x => x.Status == "Paid").Sum(x => x.Total_GST_Amount);
                nDatas.Append(WebPRNTCommands.SetText(("$" + gstTotalValue.ToString("n2")).PadLeft(20, ' ')));
            }

            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            //------------ TRANSACTION ------------
            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(shiftStatusMarker.PadRight(17, ' ') + "TRANSACTION"));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE + WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.SetText(("CASH").PadRight(20, ' ') + "No"));
            string cashCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Count().ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(cashCount));

            decimal cashValue = 0;
            if (serviceChargeInclude)
            {
                cashValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Sum(x => x.Total_Amount + x.Service_Charge);
            }
            else
            {
                cashValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 1).Sum(x => x.Total_Amount);
            }
            nDatas.Append(WebPRNTCommands.SetText(("$" + cashValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("NETS").PadRight(20, ' ') + "No"));
            string netsCount = receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Count().ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(netsCount));

            decimal netsValue = 0;
            if (serviceChargeInclude)
            {
                netsValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Sum(x => x.Total_Amount + x.Service_Charge);
            }
            else
            {
                netsValue = (decimal)receipts.Where(x => x.Status == "Paid" && x.Payment_Type == 2).Sum(x => x.Total_Amount);
            }
            nDatas.Append(WebPRNTCommands.SetText(("$" + netsValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            ComboService comboServeice = new ComboService();
            Global_Lookup_Data cardTypeLookup;

            int otherCCCount = 0;
            decimal otherCCCardValue = 0;

            foreach (var cType in receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Select(x => x.Card_Type).Distinct().ToList())
            {
                if (cType.HasValue)
                {
                    cardTypeLookup = comboServeice.GetLookup(cType.Value);

                    if (cardTypeLookup != null)
                    {
                        nDatas.Append(WebPRNTCommands.SetText(cardTypeLookup.Description.ToUpper().PadRight(20, ' ') + "No"));

                        string cardCount = receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3 && x.Card_Type == cType)
                            .Count().ToString().PadLeft(5, ' ');
                        nDatas.Append(WebPRNTCommands.SetText(cardCount));

                        var cardValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3 && x.Card_Type == cType)
                            .Sum(x => x.Payment_Amount);
                        nDatas.Append(WebPRNTCommands.SetText(("$" + cardValue.ToString("n2")).PadLeft(20, ' ')));

                        nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
                    }
                    else
                    {
                        otherCCCount += receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                            .Where(x => x.Payment_Type == 3 && x.Card_Type == cType).Count();

                        otherCCCardValue += (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                            .Where(x => x.Payment_Type == 3 && x.Card_Type == cType).Sum(x => x.Payment_Amount);
                    }
                }
                else
                {
                    otherCCCount += receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                                .Where(x => x.Payment_Type == 3).Count();

                    otherCCCardValue += (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment)
                        .Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount);
                }

            }

            if (otherCCCount > 0)
            {
                nDatas.Append(WebPRNTCommands.SetText(("OTHER CC").PadRight(20, ' ') + "No"));
                nDatas.Append(WebPRNTCommands.SetText(otherCCCount.ToString().PadLeft(5, ' ')));
                nDatas.Append(WebPRNTCommands.SetText(("$" + otherCCCardValue.ToString("n2")).PadLeft(20, ' ')));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            }

            if (surchargeInclude && surchargePercen > 0)
            {
                nDatas.Append(WebPRNTCommands.SetText(("SURCHARGES").PadRight(27, ' ')));
                var surchargeTotalValue = (decimal)receipts.Where(n => n.Status == "Paid").SelectMany(x => x.POS_Receipt_Payment).Where(x => x.Payment_Type == 3).Sum(x => x.Payment_Amount * (surchargePercen / 100));
                nDatas.Append(WebPRNTCommands.SetText(("($" + surchargeTotalValue.ToString("n2") + ")").PadLeft(21, ' ')));
                nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            }

            nDatas.Append(WebPRNTCommands.SetText(("HOLD BILL").PadRight(20, ' ') + "No"));
            string holdCount = receipts.Where(x => x.Status == "Hold").Count().ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(holdCount));

            var holdValue = (decimal)receipts.Where(x => x.Status == "Hold").Sum(x => x.Total_Amount);
            nDatas.Append(WebPRNTCommands.SetText(("$" + holdValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("VOID").PadRight(20, ' ') + "No"));
            string voidCount = receipts.Where(x => x.Status == "Void").Count().ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(voidCount));

            var voidValue = (decimal)receipts.Where(x => x.Status == "Void").Sum(x => x.Total_Amount);
            nDatas.Append(WebPRNTCommands.SetText(("$" + voidValue.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("RECEIPT").PadRight(20, ' ') + "No"));
            string receiptCount = receipts.Count().ToString().PadLeft(5, ' ');
            nDatas.Append(WebPRNTCommands.SetText(receiptCount));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.SetText(("DISPLAY TABLE#").PadRight(20, ' ') + "No"));
            nDatas.Append(WebPRNTCommands.SetText(receiptCount));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);

            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            ////------------ CATEGORY ------------
            //nDatas.Append(Commands.ALIGN_LEFT);
            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(shiftStatusMarker.PadRight(17, ' ') + "CATEGORY"));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE + WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            int categoryCount = 0;
            decimal categoryValue = 0.00m;
            int categoryTotalCount = 0;
            decimal categoryTotalAmt = 0.00m;

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

            foreach (var cat in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null && p.Product.Product_Category_L1 != null).Select(y => y.Product.Product_Category.Category_Name)).Distinct().ToList())
            {
                categoryCount = (int)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                    y => y.Product_ID != null && y.Product.Product_Category_L1 != null && y.Product.Product_Category.Category_Name == cat)).Sum(z => z.Qty);

                categoryValue = (decimal)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                    y => y.Product_ID != null && y.Product.Product_Category_L1 != null && y.Product.Product_Category.Category_Name == cat)).Sum(z => z.Price * z.Qty);

                nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace(cat.ToUpper(), categoryCount.ToString(), ("$" + categoryValue.ToString("n2")))));

                categoryTotalCount += categoryCount;
                categoryTotalAmt += categoryValue;
            }

            if (receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null && p.Product.Product_Category_L1 == null)).Count() > 0)
            {
                categoryCount = (int)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null && p.Product.Product_Category_L1 == null)).Sum(z => z.Qty);
                categoryValue = (decimal)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null && p.Product.Product_Category_L1 == null)).Sum(z => z.Price * z.Qty);

                nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace("OTHERS", categoryCount.ToString(), ("$" + categoryValue.ToString("n2")))));

                categoryTotalCount += categoryCount;
                categoryTotalAmt += categoryValue;
            }

            // OPEN ITEM CATEGORY
            if (receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Count() > 0)
            {

                categoryCount = (int)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Sum(z => z.Qty);
                categoryValue = (decimal)receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)).Sum(z => z.Price * z.Qty);

                nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace("OPEN ITEM", categoryCount.ToString(), ("$" + categoryValue.ToString("n2")))));

                categoryTotalCount += categoryCount;
                categoryTotalAmt += categoryValue;
            }

            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(("TOTAL").PadRight(22, ' ')));
            nDatas.Append(WebPRNTCommands.SetText(categoryTotalCount.ToString().PadLeft(5, ' ')));
            nDatas.Append(WebPRNTCommands.SetText(("$" + categoryTotalAmt.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            ////------------ PRODUCT ------------
            nDatas.Append(WebPRNTCommands.SetAlignment(WebPRNTCommands.Alignment.Left));
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(shiftStatusMarker.PadRight(17, ' ') + "PRODUCT"));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE + WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            int productCount = 0;
            decimal productValue = 0.00m;
            int productTotalCount = 0;
            decimal productTotalAmt = 0.00m;

            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);

            foreach (var cat in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID != null && p.Product.Product_Category_L1 != null).Select(y => y.Product.Product_Category.Category_Name)).Distinct().ToList())
            {
                foreach (var prod in receipts.SelectMany(x => x.POS_Products_Rcp.Where(y => y.Product_ID != null && y.Product.Product_Category_L1 != null && y.Product.Product_Category.Category_Name == cat).Select(y => y.Product.Product_Name)).Distinct().ToList())
                {
                    productCount = (int)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                       y => y.Product_ID != null && y.Product.Product_Category_L1 != null && y.Product.Product_Name == prod)).Sum(z => z.Qty);

                    productValue = (decimal)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                        y => y.Product_ID != null && y.Product.Product_Category_L1 != null && y.Product.Product_Name == prod)).Sum(z => z.Price * z.Qty);

                    nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace(prod.ToUpper(), productCount.ToString(), ("$" + productValue.ToString("n2")))));

                    productTotalCount += productCount;
                    productTotalAmt += productValue;
                }
            }

            foreach (var prod in receipts.SelectMany(x => x.POS_Products_Rcp.Where(y => y.Product_ID != null && y.Product.Product_Category_L1 == null).Select(y => y.Product.Product_Name)).Distinct().ToList())
            {
                productCount = (int)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                   y => y.Product_ID != null && y.Product.Product_Category_L1 == null && y.Product.Product_Name == prod)).Sum(z => z.Qty);

                productValue = (decimal)receipts.Where(x => x.Status != "Void").SelectMany(x => x.POS_Products_Rcp.Where(
                    y => y.Product_ID != null && y.Product.Product_Category_L1 == null && y.Product.Product_Name == prod)).Sum(z => z.Price * z.Qty);

                nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace(prod.ToUpper(), productCount.ToString(), ("$" + productValue.ToString("n2")))));

                productTotalCount += productCount;
                productTotalAmt += productValue;
            }

            //// OPEN ITEM PRODUCTS

            foreach (var openProduct in receipts.SelectMany(x => x.POS_Products_Rcp.Where(p => p.Product_ID == null)))
            {
                productCount = (int)openProduct.Qty;
                productValue = (decimal)(openProduct.Price * openProduct.Qty);

                nDatas.Append(WebPRNTCommands.SetText(RightAlginAddSpace(openProduct.Product_Name, productCount.ToString(), ("$" + productValue.ToString("n2")))));

                productTotalCount += productCount;
                productTotalAmt += productValue;
            }

            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, true));
            nDatas.Append(WebPRNTCommands.START_TEXT_ELEMENT);
            nDatas.Append(WebPRNTCommands.SetText(("TOTAL").PadRight(22, ' ')));
            nDatas.Append(WebPRNTCommands.SetText(productTotalCount.ToString().PadLeft(5, ' ')));
            nDatas.Append(WebPRNTCommands.SetText(("$" + productTotalAmt.ToString("n2")).PadLeft(20, ' ')));
            nDatas.Append(WebPRNTCommands.DEFAULT_NEW_LINE);
            nDatas.Append(WebPRNTCommands.END_TEXT_ELEMENT);

            nDatas.Append(WebPRNTCommands.SetCharacterSize(1, 1, false));

            nDatas.Append(WebPRNTCommands.LINE_BREAK_FULL);

            nDatas.Append(WebPRNTCommands.CutPaper(true));

            return nDatas.ToString();
        }
    }
}


public class PDFPageEvent : iTextSharp.text.pdf.PdfPageEventHelper {
    // This is the contentbyte object of the writer
    PdfContentByte cb;

    // we will put the final number of pages in a template
    PdfTemplate template;

    // this is the BaseFont we are going to use for the header / footer
    BaseFont bf = null;

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

    // write on top of document
    public override void OnOpenDocument(PdfWriter writer, Document document) {
        base.OnOpenDocument(writer, document);
        try {
            var currentdate = StoredProcedure.GetCurrentDate();
            if (PrintTime == null) {
                PrintTime = currentdate;
            }
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        } catch (DocumentException de) {
        } catch (System.IO.IOException ioe) {
        }
    }

    // write on start of each page
    public override void OnStartPage(PdfWriter writer, Document document) {
        base.OnStartPage(writer, document);
        Rectangle pageSize = document.PageSize;

        if (!string.IsNullOrEmpty(Title)) {
            cb.BeginText();
            cb.SetFontAndSize(bf, 15);
            cb.SetRGBColorFill(50, 50, 200);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
            cb.ShowText(Title);
            cb.EndText();
        }

        if (HeaderLeft + HeaderRight != string.Empty) {
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

        if (Logoleft != null) {
            Image imgleft = Image.GetInstance(Logoleft);

            if (imgleft != null) {
                if (LogoRight != null) {
                    PdfPTable table = new PdfPTable(2) {
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
                } else {
                    PdfPTable table = new PdfPTable(1) {
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
    public override void OnEndPage(PdfWriter writer, Document document) {
        base.OnEndPage(writer, document);
        int pageN = writer.PageNumber;
        String text = "Page " + pageN + " of ";
        float len = bf.GetWidthPoint(text, 8);

        Rectangle pageSize = document.PageSize;


        cb.SetRGBColorFill(100, 100, 100);

        if (Footer1 != null & Footer2 != null) {
            float f1len = bf.GetWidthPoint(Footer1, 8);
            float f2len = bf.GetWidthPoint(Footer2, 8);

            float totalwidth = (pageSize.Width) / 2;
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer1, totalwidth - (f1len / 2), pageSize.GetBottom(50), 0);
            cb.EndText();


            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer2, totalwidth - (f2len / 2), pageSize.GetBottom(30), 0);
            cb.EndText();



        } else if (Footer1 != null) {
            float f1len = bf.GetWidthPoint(Footer1, 8);

            float totalwidth = (pageSize.Width) / 2;
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Footer1, totalwidth - (f1len / 2), pageSize.GetBottom(30), 0);
            cb.EndText();
        }

        cb.BeginText();
        cb.SetFontAndSize(bf, 8);
        cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
           text,
            pageSize.GetRight(40),
            pageSize.GetBottom(30), 0);
        cb.EndText();
        cb.AddTemplate(template, pageSize.GetRight(40), pageSize.GetBottom(30));

    }

    //write on close of document
    public override void OnCloseDocument(PdfWriter writer, Document document) {
        base.OnCloseDocument(writer, document);
        template.BeginText();
        template.SetFontAndSize(bf, 8);
        template.SetTextMatrix(0, 0);
        template.ShowText("" + (writer.PageNumber - 1));
        template.EndText();
    }
}