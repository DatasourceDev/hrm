using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Authentication.Common
{
    public class DateUtil
    {
        public static string GetFullMonth(Nullable<int> month)
        {
            try
            {
                switch (month)
                {
                    case 1:
                        return "January";
                    case 2:
                        return "February";
                    case 3:
                        return "March";
                    case 4:
                        return "April";
                    case 5:
                        return "May";
                    case 6:
                        return "June";
                    case 7:
                        return "July";
                    case 8:
                        return "August";
                    case 9:
                        return "September";
                    case 10:
                        return "October";
                    case 11:
                        return "November";
                    case 12:
                        return "December";
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }
        public static string ToDisplayDate(DateTime d)
        {
            try
            {
                return d.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
        }

        public static string ToDisplayDate(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    return d.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
            }
        }

        public static string ToDisplayDateTime(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    return d.Value.ToString("dd/MM/yyyy hh:mm:ss");
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
            }
        }

        public static string ToDisplayTime(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    return d.Value.ToString("HH:mm");
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
            }
        }

        public static Nullable<DateTime> ToDate(string str)
        {
            try
            {
                if (str == null)
                {
                    return null;
                }

                int day = NumUtil.ParseInteger(str.Substring(0, 2));
                int month = NumUtil.ParseInteger(str.Substring(3, 2));
                int year = NumUtil.ParseInteger(str.Substring(6, 4));

                if (str.Contains(":"))
                {
                    int hour = 0;
                    int min = 0;
                    var dsplte = str.Split(' ');
                    if (dsplte.Length == 2)
                    {
                        dsplte = dsplte[1].Split(':');
                        if (dsplte.Length == 2)
                        {
                            hour = NumUtil.ParseInteger(dsplte[0]);
                            min = NumUtil.ParseInteger(dsplte[1]);
                            if (string.IsNullOrEmpty(dsplte[0]) && string.IsNullOrEmpty(dsplte[0]))
                            {
                                str = str.Replace(":", "").Trim();
                            }
                        }
                    }

                    var d = DateTime.ParseExact(str, "dd/MM/yyyy HH:mm", null);
                    if (d.Year < 1500)
                    {
                        year += 543;
                        str = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString() + " " + hour.ToString("00") + ":" + min.ToString("00");
                    }
                    else if (d.Year > 2500)
                    {
                        year -= 543;
                        str = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString() + " " + hour.ToString("00") + ":" + min.ToString("00");
                    }

                    return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm", null);
                }
                else
                {

                    var d = DateTime.ParseExact(str, "dd/MM/yyyy", null);
                    if (d.Year < 1500)
                    {
                        year += 543;
                        str = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString();
                    }
                    else if (d.Year > 2500)
                    {
                        year -= 543;
                        str = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString();
                    }
                    return DateTime.ParseExact(str, "dd/MM/yyyy", null);
                }
            }
            catch
            {
                return null;
            }
        }

        public static Nullable<DateTime> ToDate(int day, int month, int year)
        {
            try
            {
                if (day == 0 || month == 0 || year == 0)
                {
                    return null;
                }
                var datestr = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString();
                var d = DateTime.ParseExact(datestr, "dd/MM/yyyy", null);
                if (d.Year < 1500)
                {
                    year += 543;
                    datestr = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString();
                }
                else if (d.Year > 2500)
                {
                    year -= 543;
                    datestr = day.ToString("00") + "/" + month.ToString("00") + "/" + year.ToString();
                }
                return DateTime.ParseExact(datestr, "dd/MM/yyyy", null);
            }
            catch
            {
                return null;
            }
        }


    }
}