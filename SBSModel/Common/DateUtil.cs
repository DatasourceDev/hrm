using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SBSResourceAPI;

namespace SBSModel.Common
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
                        return Resource.January;
                    case 2:
                        return Resource.February;
                    case 3:
                        return Resource.March;
                    case 4:
                        return Resource.April;
                    case 5:
                        return Resource.May;
                    case 6:
                        return Resource.June;
                    case 7:
                        return Resource.July;
                    case 8:
                        return Resource.August;
                    case 9:
                        return Resource.September;
                    case 10:
                        return Resource.October;
                    case 11:
                        return Resource.November;
                    case 12:
                        return Resource.December;
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }
        public static string GetFullDayOfweek(Nullable<int> dayOfweek)
        {
            try
            {
           
                switch (dayOfweek)
                {
                    case -1:
                        return Resource.ALL;
                    case 0:
                        return Resource.Sunday;
                    case 1:
                        return Resource.Monday;
                    case 2:
                        return Resource.Tuesday;
                    case 3:
                        return Resource.Wednesday;
                    case 4:
                        return Resource.Thursday;
                    case 5:
                        return Resource.Friday;
                    case 6:
                        return Resource.Saturday;
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
                CultureInfo UsaCulture = new CultureInfo("en-US");
                return d.ToString("dd/MM/yyyy", UsaCulture);
            }
            catch
            {
                return "";
            }
        }

        public static string ToDisplayDDMMMYYYY(DateTime d)
        {
            try
            {
                CultureInfo UsaCulture = new CultureInfo("en-US");
                return d.ToString("dd MMM yyyy", UsaCulture);
            }
            catch
            {
                return "";
            }
        }

        public static string ToDisplayDate(Nullable<DateTime> d, string indicator = "/")
        {
            try
            {
                if (d.HasValue)
                {
                    CultureInfo UsaCulture = new CultureInfo("en-US");
                    return d.Value.ToString("dd" + indicator + "MM" + indicator + "yyyy", UsaCulture);
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

        public static string ToDisplayDate2(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    return d.Value.ToString("dd.MM.yyyy");
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

        public static string ToDisplayFullDayOfweek(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    CultureInfo UsaCulture = new CultureInfo("en-US");
                    return d.Value.ToString("dddd dd MMM yyyy", UsaCulture);
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
        public static string GetDayOfweek(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    CultureInfo UsaCulture = new CultureInfo("en-US");
                    return d.Value.ToString("dddd", UsaCulture);
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
       
        public static string ToDisplayFullDate(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    CultureInfo UsaCulture = new CultureInfo("en-US");
                    return d.Value.ToString("dd MMM yyyy", UsaCulture);
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
        public static string ToDisplayFullDateTime(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    CultureInfo UsaCulture = new CultureInfo("en-US");
                    return d.Value.ToString("dd MMM yyyy HH:mm:ss", UsaCulture);
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
                    return d.Value.ToString("dd/MM/yyyy HH:mm:ss");
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

        public static string ToDisplayTime(Nullable<System.TimeSpan> t)
        {
            try
            {
                if (t.HasValue)
                {
                    var h = t.Value.Hours.ToString("00");
                    var m = t.Value.Minutes.ToString("00");

                    return h + ":" + m;
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

        public static String ToLocalTime(DateTime dt)
        {            
            try
            {                
                return dt.ToLocalTime().ToShortTimeString();

            }
            catch(FormatException)
            {
                return dt.ToShortTimeString();
            }
        }

        public static Nullable<DateTime> ToDate(string str, string indicator = "/")
        {
            try
            {
                if (str == null)
                    return null;


                int day = 0;
                int month = 0;
                int year = 0;

                string[] indicators = { indicator };
                var datesplit = str.Split(indicators, StringSplitOptions.RemoveEmptyEntries);
                if (datesplit.Length == 3)
                {
                    day = NumUtil.ParseInteger(datesplit[0]);
                    month = NumUtil.ParseInteger(datesplit[1]);
                    year = NumUtil.ParseInteger(datesplit[2].Substring(0, 4));
                }


                if (str.Contains(":"))
                {
                    int hour = 0;
                    int min = 0;
                    int sec = 0;
                    var dsplte = str.Split(' ');
                    if (dsplte.Length == 2)
                    {
                        dsplte = dsplte[1].Split(':');
                        if (dsplte.Length == 2)
                        {
                            hour = NumUtil.ParseInteger(dsplte[0]);
                            min = NumUtil.ParseInteger(dsplte[1]);
                            if (string.IsNullOrEmpty(dsplte[0]) && string.IsNullOrEmpty(dsplte[1]))
                            {
                                str = str.Replace(":", "").Trim();
                            }
                            //var d = DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy HH:mm", null);
                            if (year < 1500)
                                year += 543;
                            else if (year > 2500)
                                year -= 543;

                            str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString() + " " + hour.ToString("00") + ":" + min.ToString("00");

                            return DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy HH:mm", new CultureInfo("en-US", false));
                        }
                        else if (dsplte.Length == 3)
                        {
                            hour = NumUtil.ParseInteger(dsplte[0]);
                            min = NumUtil.ParseInteger(dsplte[1]);
                            sec = NumUtil.ParseInteger(dsplte[2]);
                            if (string.IsNullOrEmpty(dsplte[0]) && string.IsNullOrEmpty(dsplte[1]) && string.IsNullOrEmpty(dsplte[2]))
                            {
                                str = str.Replace(":", "").Trim();
                            }
                            //var d = DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy HH:mm:ss", null);

                            if (year < 1500)
                            {
                                year += 543;
                                str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString() + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");
                            }
                            else if (year > 2500)
                            {
                                year -= 543;
                                str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString() + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");
                            }

                            return DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy HH:mm:ss", new CultureInfo("en-US", false));
                        }

                    }


                }
                else
                {
                    str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                    var d = DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy", new CultureInfo("en-US", false));
                    if (d.Year < 1500)
                    {
                        year += 543;
                        str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                    }
                    else if (d.Year > 2500)
                    {
                        year -= 543;
                        str = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                    }
                    return DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy", new CultureInfo("en-US", false));
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public static Nullable<DateTime> ToDate(int day, int month, int year, string indicator = "/")
        {
            try
            {
                if (day == 0 || month == 0 || year == 0)
                {
                    return null;
                }
                var datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                var d = DateTime.ParseExact(datestr, "dd/MM/yyyy", new CultureInfo("en-US", false));
                if (d.Year < 1500)
                {
                    year += 543;
                    datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                }
                else if (d.Year > 2500)
                {
                    year -= 543;
                    datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                }
                return DateTime.ParseExact(datestr, "dd" + indicator + "MM" + indicator + "yyyy", new CultureInfo("en-US", false));
            }
            catch
            {
                return null;
            }
        }

        public static Nullable<DateTime> ToDate(int day, int month, int year, int hour, int minute, int second, string indicator = "/")
        {
            try
            {
                if (day == 0 || month == 0 || year == 0)
                {
                    return null;
                }
                var datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();

                var timeStr = hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");

                var d = DateTime.ParseExact(datestr, "dd/MM/yyyy", new CultureInfo("en-US", false));

                if (d.Year < 1500)
                {
                    year += 543;
                    datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                }
                else if (d.Year > 2500)
                {
                    year -= 543;
                    datestr = day.ToString("00") + indicator + month.ToString("00") + indicator + year.ToString();
                }
                return DateTime.ParseExact(datestr + " " + timeStr, "dd" + indicator + "MM" + indicator + "yyyy" + " HH:mm:ss", new CultureInfo("en-US", false));
            }
            catch
            {
                return null;
            }
        }

        public static Nullable<TimeSpan> ToTime(string str)
        {
            try
            {
                if (str == null)
                {
                    return null;
                }

                str = str.Replace(":", ".");
                var d = TimeSpan.ParseExact(str, "hh'.'mm", new CultureInfo("en-US", false));

                return d;
            }
            catch
            {
                return null;
            }
        }
        public static double WorkDays(DateTime? sdate, DateTime? edate, List<int> workingDays, List<DateTime> holidays = null)
        {
            double returnDays = 0;
            if (sdate.HasValue && edate.HasValue)
            {
                var firstDay = sdate.Value;
                var lastDay = edate.Value;
                while (firstDay <= lastDay)
                {
                    int firstDayOfWeek = (int)firstDay.DayOfWeek;
                    if (workingDays.Contains(firstDayOfWeek))
                    {
                        if (holidays != null)
                        {
                            if (!holidays.Contains(firstDay))
                                returnDays++;
                        }
                        else
                            returnDays++;
                    }
                    firstDay = firstDay.AddDays(1);
                }
            }
            return returnDays;
        }


        public static double WorkDays(int year, int month, List<int> workingDays, List<DateTime> holidays = null)
        {
            double returnDays = 0;

            if (ToDate(1, month, year) == null) return 0;

            var firstDay = ToDate(1, month, year).Value;
            var lastDay = ToDate(DateTime.DaysInMonth(year, month), month, year).Value;

            while (firstDay <= lastDay)
            {
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                if (workingDays.Contains(firstDayOfWeek))
                {
                    if (holidays != null)
                    {
                        if (!holidays.Contains(firstDay))
                            returnDays++;
                    }
                    else
                        returnDays++;
                }
                firstDay = firstDay.AddDays(1);
            }
            return returnDays;
        }

        
        
        public static string ToDisplayDateRange(Nullable<DateTime> sdate, string speriod, Nullable<DateTime> edate, string eperiod)
        {
            return ToDisplayDateRange(ToDisplayDate(sdate), speriod, ToDisplayDate(edate), eperiod);
        }

        public static string ToDisplayDateRange(string sdate, string speriod, string edate, string eperiod)
        {
            var str = "";
            if (!string.IsNullOrEmpty(sdate) & !string.IsNullOrEmpty(edate))
            {
                str += sdate;
                if (!string.IsNullOrEmpty(speriod))
                {
                    str += " " + speriod;
                }
                str += " to ";
                if (string.IsNullOrEmpty(edate))
                {
                    edate = sdate;
                    eperiod = speriod;
                }

                str += edate;
                if (!string.IsNullOrEmpty(eperiod))
                {
                    str += " " + eperiod;
                }
            }
            else if (!string.IsNullOrEmpty(sdate))
            {
                str += sdate;
                if (!string.IsNullOrEmpty(speriod))
                {
                    str += " " + speriod;
                }
            }
            else if (!string.IsNullOrEmpty(edate))
            {
                str += edate;
                if (!string.IsNullOrEmpty(eperiod))
                {
                    str += " " + eperiod;
                }
            }
            return str;
        }

        public static string ToInternalDate(Nullable<DateTime> d)
        {
            try
            {
                if (d.HasValue)
                {
                    return d.Value.ToString("yyyy-MM-dd HH:mm:ss");
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

        public static string ToInternalDate(string str, string indicator = "/")
        {
            try
            {
                if (str == null)
                    return null;

                int day = 0;
                int month = 0;
                int year = 0;
                var datestr = "";
                string[] indicators = { indicator };
                var datesplit = str.Split(indicators, StringSplitOptions.RemoveEmptyEntries);
                if (datesplit.Length == 3)
                {
                    datestr = datesplit[0] + "/" + datesplit[1] + "/" + datesplit[2].Substring(0, 4);
                    day = NumUtil.ParseInteger(datesplit[0]);
                    month = NumUtil.ParseInteger(datesplit[1]);
                    year = NumUtil.ParseInteger(datesplit[2].Substring(0, 4));


                }

                if (str.Contains(":"))
                {
                    int hour = 0;
                    int min = 0;
                    int sec = 0;

                    var newstr = str.Replace(datestr, "").Trim();
                    var dsplte = newstr.Split(':');

                    if (dsplte.Length == 2)
                    {
                        hour = NumUtil.ParseInteger(dsplte[0]);
                        min = NumUtil.ParseInteger(dsplte[1]);
                        if (string.IsNullOrEmpty(dsplte[0]) && string.IsNullOrEmpty(dsplte[1]))
                        {
                            str = str.Replace(":", "").Trim();
                        }
                        //var d = DateTime.ParseExact(str, "dd" + indicator + "MM" + indicator + "yyyy HH:mm", null);
                        if (year < 1500)
                            year += 543;
                        else if (year > 2500)
                            year -= 543;

                        str = year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00");
                        return str;
                    }
                    else if (dsplte.Length == 3)
                    {
                        hour = NumUtil.ParseInteger(dsplte[0]);
                        min = NumUtil.ParseInteger(dsplte[1]);
                        sec = NumUtil.ParseInteger(dsplte[2]);
                        if (string.IsNullOrEmpty(dsplte[0]) && string.IsNullOrEmpty(dsplte[1]) && string.IsNullOrEmpty(dsplte[2]))
                        {
                            str = str.Replace(":", "").Trim();
                        }

                        if (year < 1500)
                            year += 543;
                        else if (year > 2500)
                            year -= 543;


                        str = year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");
                        return str;
                    }
                }
                else
                {
                    if (year < 1500)
                        year += 543;
                    else if (year > 2500)
                        year -= 543;

                    str = year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00");
                    return str;
                }
            }
            catch
            {
                return "";
            }

            return "";
        }
    }
}