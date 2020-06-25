using Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace HR.Common
{

    public enum ColType
    {
        colString,
        colDateTime
    }

    public class XmlUtil
    {

        StringBuilder _strColStyle = new StringBuilder();
        StringBuilder _strAddCol = new StringBuilder();

        public void stmWRWrite(StreamWriter stmWR, string Tag)
        {
            stmWR.Write(Tag);
        }

        public void OpenSheet(StreamWriter stmWR, int sheetNo, string sheetName, int columnCnt, double width)
        {
            stmWRWrite(stmWR, TAG_OPEN_WORKSHEET.Replace("#NAME", sheetName));
            stmWRWrite(stmWR, "<Names>");
            stmWRWrite(stmWR, "<NamedRange ss:Name=\"_FilterDatabase\" ss:RefersTo=\"=" + sheetName + "!#REF!\" ss:Hidden=\"1\"/>");
            stmWRWrite(stmWR, "</Names>");
            stmWRWrite(stmWR, TAG_OPEN_TABLE(columnCnt));

            for (var i = 0; i < columnCnt; i++ )
                stmWRWrite(stmWR, "<Column ss:AutoFitWidth=\"0\" ss:Width=\"" + width + "\"/>");

        }
        public void CloseSheet(StreamWriter stmWR)
        {
            stmWRWrite(stmWR, TAG_CLOSE_TABLE);
            stmWRWrite(stmWR, WORKSHEET_OPTION);
            stmWRWrite(stmWR, TAG_CLOSE_WORKSHEET);
        }
        public string CreateTAGData(string _strType, string _strData, int colspan =1)
        {
            var sb = new StringBuilder();

            decimal dec = 0;double dob = 82;DateTime date = new DateTime(); int i = 0;string s = "";
            
            if (_strType == s.GetType().ToString())
            {
                if (colspan > 1)
                    sb.Append("<Cell ss:MergeAcross=\"" + (colspan -1) + "\">");
                else
                    sb.Append("<Cell>");

                sb.Append("<Data ss:Type=\"String\" x:Ticked=\"1\">");

            }
            else if (_strType == dec.GetType().ToString() || _strType == dob.GetType().ToString())
            {
                if (colspan > 1)
                    sb.Append("<Cell ss:StyleID=\"s27\" ss:MergeAcross=\"" + (colspan - 1) + "\">");
                else
                    sb.Append("<Cell ss:StyleID=\"s27\">");

                sb.Append("<Data ss:Type=\"Number\">");
            }
            else if (_strType == date.GetType().ToString())
            {
                if (colspan > 1)
                    sb.Append("<Cell ss:StyleID=\"s26\" ss:MergeAcross=\"" + (colspan - 1) + "\">");
                else
                    sb.Append("<Cell ss:StyleID=\"s26\">");

                sb.Append("<Data ss:Type=\"DateTime\">");
            }
            else if (_strType == i.GetType().ToString())
            {
                if (colspan > 1)
                    sb.Append("<Cell ss:MergeAcross=\"" + (colspan - 1) + "\">");
                else
                    sb.Append("<Cell>");

                sb.Append("<Data ss:Type=\"Number\">");
            }
            else
            {
                if (colspan > 1)
                    sb.Append("<Cell ss:MergeAcross=\"" + (colspan - 1) + "\">");
                else
                    sb.Append("<Cell>");

                sb.Append("<Data ss:Type=\"String\" x:Ticked=\"1\">");
            }

            sb.Append(CheckData(_strData));
            sb.Append(TAG_CLOSE_CELL);

            return sb.ToString();
        }
      

        public string CheckData(string data)
        {
            var strData = "";
            strData = data.Trim();
            strData = strData.Replace("&", "&amp;");
            strData = strData.Replace("<", "&lt;");
            strData = strData.Replace(">", "&gt;");
            strData = strData.Replace("\"", "&quot;");
            return strData;
        }

       

        public void BeginAddColoumn()
        {
            _strAddCol = new StringBuilder();
            _strColStyle = new StringBuilder();
            _strAddCol.Append(TAG_FIRST_CELL);
        }

        public void EndAddColoumn(StringBuilder _sb)
        {
            _strAddCol.Append(TAG_CLOSE_ROW);
            _sb.Append(_strColStyle.ToString());
            _sb.Append(_strAddCol.ToString());
        }
        public void AddExcelColumn(string _colName, double _width, ColType _colType)
        {
            if (_colType == ColType.colString)
            {
                _strColStyle.Append(TAG_OPEN_STYLE + _width.ToString() + TAG_CLOSE_STYLE);
            }
            if (_colType == ColType.colDateTime)
            {
                _strAddCol.Append(TAG_OPEN_CELL_DATETIME + _colName + TAG_CLOSE_CELL);
            }
            else
            {
                _strAddCol.Append(TAG_OPEN_CELL_NORMAL + _colName + TAG_CLOSE_CELL);
            }
        }
       

        public string START_EXCEL = "<?xml version=\"1.0\"?>"
  + "<?mso-application progid=\"Excel.Sheet\"?>"
  + "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\""
  + " xmlns:o=\"urn:schemas-microsoft-com:office:office\""
  + " xmlns:x=\"urn:schemas-microsoft-com:office:excel\""
  + " xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\""
  + " xmlns:html=\"http://www.w3.org/TR/REC-html40\">"
  + "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">"
  + "<WindowHeight>11640</WindowHeight>"
  + "<WindowWidth>15480</WindowWidth>"
  + "<WindowTopX>0</WindowTopX>"
  + "<WindowTopY>120</WindowTopY>"
  + "<ProtectStructure>False</ProtectStructure>"
  + "<ProtectWindows>False</ProtectWindows>"
  + "</ExcelWorkbook>"
  + "<Styles>"
  + "<Style ss:ID=\"Default\" ss:Name=\"Normal\">"
  + "<Alignment ss:Vertical=\"Bottom\"/>"
  + "<Borders/>"
  + "<Font/>"
  + "<Interior/>"
  + "<NumberFormat/>"
  + "<Protection/>"
  + "</Style>"
  + "<Style ss:ID=\"s23\">"
  + "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>"
  + "</Style>"
  + "<Style ss:ID=\"s24\">"
  + "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>"
  + "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>"
  + "<NumberFormat ss:Format=\"[THA]dd/mm/yyyy\"/>"
  + "</Style>"
  + "<Style ss:ID=\"s26\">"
  + "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>"
  + "<NumberFormat ss:Format=\"[THA]dd/mm/yyyy\"/>"
  + "</Style>"
  + "<Style ss:ID=\"s27\">"
  + "<NumberFormat ss:Format=\"Fixed\"/>"
  + "</Style>"
  + "</Styles>";

        public string WORKSHEET_OPTION = "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">"
           + "<Print>"
           + "<ValidPrinterInfo/>"
           + "<PaperSizeIndex>9</PaperSizeIndex>"
           + "<HorizontalResolution>600</HorizontalResolution>"
           + "<VerticalResolution>0</VerticalResolution>"
           + "</Print>"
           + "<Selected/>"
           + "<FreezePanes/>"
           + "<FrozenNoSplit/>"
           + "<SplitHorizontal>1</SplitHorizontal>"
           + "<TopRowBottomPane>1</TopRowBottomPane>"
           + "<ActivePane>2</ActivePane>"
           + "<Panes>"
           + "<Pane>"
           + "<Number>3</Number>"
           + "</Pane>"
           + "<Pane>"
           + "<Number>2</Number>"
           + "<RangeSelection>R1:R2</RangeSelection>"
           + "</Pane>"
           + "</Panes>"
           + "<ProtectObjects>False</ProtectObjects>"
           + "<ProtectScenarios>False</ProtectScenarios>"
           + "</WorksheetOptions>";

        public string TAG_OPEN_WORKSHEET = "<Worksheet ss:Name=\"#NAME\">";
        public string TAG_CLOSE_WORKSHEET = "</Worksheet>";

        public string TAG_OPEN_TABLE(int columnCount)
        {
            return "<Table ss:ExpandedColumnCount=\"COLUMN_COUNT\" ss:ExpandedRowCount=\"65535\" x:FullColumns=\"1\" x:FullRows=\"1\">".Replace("COLUMN_COUNT", columnCount.ToString());
        }

       
        public string TAG_CLOSE_TABLE = "</Table>";
        public string TAG_CLOSE_WORKBOOK = "</Workbook>";


        public string TAG_OPEN_ROW = "<Row>";
        public string TAG_CLOSE_ROW = "</Row>";
        public string TAG_CLOSE_CELL = "</Data></Cell>";
        public string TAG_OPEN_CELL = "</Data></Cell>";
        public string TAG_OPEN_STYLE = "<Column ss:AutoFitWidth=\"0\" ss:Width=\"";
        public string TAG_CLOSE_STYLE = "\"/>";
        public string TAG_OPEN_CELL_NORMAL = "<Cell><Data ss:Type=\"String\">";
        public string TAG_OPEN_CELL_DATETIME = "<Cell ss:StyleID=\"s24\"><Data ss:Type=\"String\">";
        public string TAG_FIRST_CELL = "<Row ss:StyleID=\"s23\">";
    }
}