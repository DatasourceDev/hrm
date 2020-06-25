using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBSModel.Common
{
    public class DateCal
    {
        public static DateTime GetEndDate(DateTime firstDay, decimal daystaken, double workingDays, List<DateTime> bankHolidays)
        {
            if (bankHolidays == null)
                bankHolidays = new List<DateTime>();

            if (workingDays == WorkingDays.Days5)
            {
                var i = 0;
                var day = firstDay;
                while (i < daystaken)
                {
                    int firstDayOfWeek = (int)day.DayOfWeek;
                    if (firstDayOfWeek <= 5 && firstDayOfWeek > 0)
                    {
                        var isholiday = false;
                        if (bankHolidays.Contains(day))
                            isholiday = true;
                        if (!isholiday)
                            i++;
                    }
                    if (i < daystaken)
                        day = day.AddDays(1);
                }
                return day;
            }
            else if (workingDays == WorkingDays.Days6)
            {
                var i = 0;
                var day = firstDay;
                while (i < daystaken)
                {
                    int firstDayOfWeek = (int)day.DayOfWeek;
                    if (firstDayOfWeek <= 6 && firstDayOfWeek > 0)
                    {
                        var isholiday = false;
                        if (bankHolidays.Contains(day))
                            isholiday = true;
                        if (!isholiday)
                            i++;

                    }
                    if (i < daystaken)
                        day = day.AddDays(1);
                }
                return day;
            }
            else
            {
                /*WorkingDays 7*/
                var i = 0;
                var day = firstDay;
                while (i < daystaken)
                {
                    var isholiday = false;
                    if (bankHolidays.Contains(day))
                        isholiday = true;
                    if (!isholiday)
                        i++;

                    if (i < daystaken)
                        day = day.AddDays(1);
                }
                return day;
            }
        }

        public static double BusinessDaysUntil(DateTime firstDay, string firstPeriod, Nullable<DateTime> lastDay, string lastPeriod, List<int> workingDays, List<DateTime> bankHolidays)
        {
            if (bankHolidays == null)
                bankHolidays = new List<DateTime>();

            if (workingDays == null)
               workingDays = new List<int>();

            firstDay = firstDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);
            double returnDays = 0;
            var curDay = firstDay;
            if (lastDay.HasValue)
            {
             
                while (curDay <= lastDay)
                {
                    int curDayOfWeek = (int)curDay.DayOfWeek;
                    int lastDayOfWeek = (int)lastDay.Value.DayOfWeek;
                    if (workingDays.Contains(curDayOfWeek))
                    {
                        if (!bankHolidays.Contains(curDay))
                            returnDays += 1;
                    }

                    curDay = curDay.AddDays(1);
                }


                if (firstDay == lastDay &&  firstPeriod == lastPeriod && firstPeriod == Period.PM)
                {
                    int firstDayOfWeek = (int)firstDay.DayOfWeek;
                    if (workingDays.Contains(firstDayOfWeek) && !bankHolidays.Contains(firstDay))
                        returnDays -= 0.5;
                }
                if (firstDay == lastDay &&  firstPeriod == lastPeriod && lastPeriod == Period.AM)
                {
                    int lastDayOfWeek = (int)lastDay.Value.DayOfWeek;
                    if (workingDays.Contains(lastDayOfWeek) && !bankHolidays.Contains(lastDay.Value))
                        returnDays -= 0.5;
                }
                if (firstDay != lastDay && firstPeriod == Period.PM)
                {
                    int firstDayOfWeek = (int)firstDay.DayOfWeek;
                    if (workingDays.Contains(firstDayOfWeek) && !bankHolidays.Contains(firstDay))
                        returnDays -= 0.5;
                }
                if (firstDay != lastDay && lastPeriod == Period.AM)
                {
                    int firstDayOfWeek = (int)firstDay.DayOfWeek;
                    if (workingDays.Contains(firstDayOfWeek) && !bankHolidays.Contains(firstDay))
                        returnDays -= 0.5;
                }
            }
            else
            {
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                if (workingDays.Contains(firstDayOfWeek))
                {
                    if (!bankHolidays.Contains(firstDay))
                        returnDays += 1;
                }

                if (firstPeriod == Period.PM)
                {
                    if (workingDays.Contains(firstDayOfWeek) && !bankHolidays.Contains(firstDay))
                        returnDays -= 0.5;
                }
            }
            return returnDays;
        }


        //public static double BusinessDaysUntil(DateTime firstDay, string firstPeriod, Nullable<DateTime> lastDay, string lastPeriod, double workingDays, List<DateTime> bankHolidays)
        //{
        //    if (bankHolidays == null)
        //        bankHolidays = new List<DateTime>();


        //    firstDay = firstDay.Date;
        //    if (firstDay > lastDay)
        //        throw new ArgumentException("Incorrect last day " + lastDay);
        //    if (lastDay.HasValue)
        //    {
        //        TimeSpan span = lastDay.Value - firstDay;
        //        int businessDays = span.Days + 1;
        //        int fullWeekCount = businessDays / 7;
        //        double returnDays = businessDays;
        //        if (workingDays == WorkingDays.Days5_5)
        //        {
        //            // find out if there are weekends during the time exceedng the full weeks
        //            if (businessDays > fullWeekCount * 7)
        //            {
        //                // we are here to find out if there is a 1-day or 2-days weekend
        //                // in the time interval remaining after subtracting the complete weeks
        //                int firstDayOfWeek = (int)firstDay.DayOfWeek;
        //                int lastDayOfWeek = (int)lastDay.Value.DayOfWeek;
        //                if (lastDayOfWeek < firstDayOfWeek) // ข้ามอาทิตย์
        //                    lastDayOfWeek += 7;
        //                if (firstDayOfWeek == 0)
        //                {
        //                    returnDays -= 1;
        //                }
        //                else if (firstDayOfWeek <= 6)// วันแรก จันทร์ - เสาร์
        //                {
        //                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval วันสุดท้ายตก อาทิตย์ -> ข้ามอาทิตย์
        //                        returnDays -= 1.5;
        //                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval วันสุดท้ายตกวันเสาร์
        //                        returnDays -= 0.5;
        //                }
        //                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval วันอาทิตย์
        //                    returnDays -= 1;
        //            }

        //            //subtract the weekends during the full weeks in the interval
        //            double full = fullWeekCount;
        //            returnDays -= (full / 2) + fullWeekCount;

        //            // subtract the number of bank holidays during the time interval

        //            foreach (DateTime bankHoliday in bankHolidays)
        //            {
        //                if ((int)bankHoliday.DayOfWeek == 6)
        //                {
        //                    DateTime bh = bankHoliday.Date;
        //                    if (firstDay <= bh && bh <= lastDay)
        //                        returnDays -= 0.5;
        //                }
        //                else if ((int)bankHoliday.DayOfWeek != 0)
        //                {
        //                    DateTime bh = bankHoliday.Date;
        //                    if (firstDay <= bh && bh <= lastDay)
        //                        --returnDays;
        //                }

        //            }
        //            if ((int)firstDay.DayOfWeek == 6)
        //            {

        //            }
        //            else if ((int)firstDay.DayOfWeek != 0)
        //            {
        //                if (firstPeriod == Period.PM & !bankHolidays.Contains(firstDay.Date))
        //                    returnDays -= 0.5;
        //            }

        //            if ((int)lastDay.Value.DayOfWeek == 6)
        //            {

        //            }
        //            else if ((int)lastDay.Value.DayOfWeek != 0)
        //            {
        //                if (lastPeriod == Period.AM & !bankHolidays.Contains(lastDay.Value.Date))
        //                    returnDays -= 0.5;
        //            }

        //        }
        //        else if (workingDays == WorkingDays.Days6)
        //        {
        //            // find out if there are weekends during the time exceedng the full weeks
        //            if (businessDays > fullWeekCount * 7)
        //            {
        //                // we are here to find out if there is a 1-day or 2-days weekend
        //                // in the time interval remaining after subtracting the complete weeks
        //                int firstDayOfWeek = (int)firstDay.DayOfWeek;
        //                int lastDayOfWeek = (int)lastDay.Value.DayOfWeek;
        //                if (lastDayOfWeek < firstDayOfWeek) // ข้ามอาทิตย์
        //                    lastDayOfWeek += 7;
        //                if (firstDayOfWeek == 0)
        //                {
        //                    returnDays -= 1;
        //                }
        //                else if (firstDayOfWeek <= 6)// วันแรก จันทร์ - เสาร์
        //                {
        //                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval วันสุดท้ายตก อาทิตย์ -> ข้ามอาทิตย์
        //                        returnDays -= 1;
        //                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval วันสุดท้ายตกวันเสาร์
        //                        returnDays -= 0;
        //                }
        //                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval วันอาทิตย์
        //                    returnDays -= 1;
        //            }

        //            //subtract the weekends during the full weeks in the interval
        //            returnDays -= fullWeekCount;
        //            // subtract the number of bank holidays during the time interval
        //            foreach (DateTime bankHoliday in bankHolidays)
        //            {
        //                if ((int)bankHoliday.DayOfWeek != 0)
        //                {
        //                    DateTime bh = bankHoliday.Date;
        //                    if (firstDay <= bh && bh <= lastDay)
        //                        --returnDays;
        //                }
        //            }

        //            if (firstPeriod == Period.PM)
        //            {
        //                if ((int)firstDay.DayOfWeek != 0 & !bankHolidays.Contains(firstDay.Date))
        //                {
        //                    returnDays -= 0.5;

        //                }
        //            }
        //            if (lastPeriod == Period.AM)
        //            {
        //                if ((int)lastDay.Value.DayOfWeek != 0 & !bankHolidays.Contains(lastDay.Value.Date))
        //                    returnDays -= 0.5;
        //            }
        //        }
        //        else if (workingDays == WorkingDays.Days7)
        //        {
        //            foreach (DateTime bankHoliday in bankHolidays)
        //            {
        //                if ((int)bankHoliday.DayOfWeek != 0)
        //                {
        //                    DateTime bh = bankHoliday.Date;
        //                    if (firstDay <= bh && bh <= lastDay)
        //                        --returnDays;
        //                }
        //            }

        //            if (firstPeriod == Period.PM)
        //            {
        //                if ((int)firstDay.DayOfWeek != 0 & !bankHolidays.Contains(firstDay.Date))
        //                {
        //                    returnDays -= 0.5;

        //                }
        //            }
        //            if (lastPeriod == Period.AM)
        //            {
        //                if ((int)lastDay.Value.DayOfWeek != 0 & !bankHolidays.Contains(lastDay.Value.Date))
        //                    returnDays -= 0.5;
        //            }
        //        }
        //        else
        //        {
        //            // find out if there are weekends during the time exceedng the full weeks
        //            if (businessDays > fullWeekCount * 7)
        //            {
        //                // we are here to find out if there is a 1-day or 2-days weekend
        //                // in the time interval remaining after subtracting the complete weeks
        //                int firstDayOfWeek = (int)firstDay.DayOfWeek;
        //                int lastDayOfWeek = (int)lastDay.Value.DayOfWeek;
        //                if (lastDayOfWeek < firstDayOfWeek) // ข้ามอาทิตย์
        //                    lastDayOfWeek += 7;
        //                if (firstDayOfWeek == 0)
        //                {
        //                    returnDays -= 1;
        //                }
        //                else if (firstDayOfWeek <= 6)// วันแรก จันทร์ - เสาร์
        //                {
        //                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval วันสุดท้ายตก อาทิตย์ -> ข้ามอาทิตย์
        //                        returnDays -= 2;
        //                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval วันสุดท้ายตกวันเสาร์
        //                        returnDays -= 1;
        //                }
        //                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval วันอาทิตย์
        //                    returnDays -= 1;
        //            }

        //            //subtract the weekends during the full weeks in the interval
        //            returnDays -= fullWeekCount + fullWeekCount;
        //            // subtract the number of bank holidays during the time interval
        //            foreach (DateTime bankHoliday in bankHolidays)
        //            {
        //                if ((int)bankHoliday.DayOfWeek != 0 & (int)bankHoliday.DayOfWeek != 6)
        //                {
        //                    DateTime bh = bankHoliday.Date;
        //                    if (firstDay <= bh && bh <= lastDay)
        //                        --returnDays;
        //                }
        //            }

        //            if (firstPeriod == Period.PM)
        //            {
        //                if ((int)firstDay.DayOfWeek != 0 & (int)firstDay.DayOfWeek != 6 & !bankHolidays.Contains(firstDay.Date))
        //                {
        //                    returnDays -= 0.5;

        //                }
        //            }
        //            if (lastPeriod == Period.AM)
        //            {
        //                if ((int)lastDay.Value.DayOfWeek != 0 & (int)lastDay.Value.DayOfWeek != 6 & !bankHolidays.Contains(lastDay.Value.Date))
        //                    returnDays -= 0.5;
        //            }
        //        }
        //        return returnDays;
        //    }
        //    else
        //    {

        //        if (workingDays == WorkingDays.Days5_5)
        //        {
        //            if ((int)firstDay.DayOfWeek == 0)
        //                return 0;
        //        }
        //        else if (workingDays == WorkingDays.Days6)
        //        {
        //            if ((int)firstDay.DayOfWeek == 0)
        //                return 0;
        //        }
        //        else if (workingDays == WorkingDays.Days7)
        //        {
        //        }
        //        else
        //        {
        //            if ((int)firstDay.DayOfWeek == 0 | (int)firstDay.DayOfWeek == 6)
        //                return 0;
        //        }

        //        if (firstPeriod == Period.AM | firstPeriod == Period.PM)
        //        {
        //            double returndays = 0.5;
        //            if ((int)firstDay.DayOfWeek == 6 & firstPeriod == Period.PM & workingDays == WorkingDays.Days5_5)
        //                return 0;

        //            //subtract the number of bank holidays during the time interval
        //            if (bankHolidays.Contains(firstDay.Date))
        //                return 0;

        //            return returndays;
        //        }
        //        else
        //        {
        //            double returndays = 1;
        //            if ((int)firstDay.DayOfWeek == 6 & workingDays == WorkingDays.Days5_5)
        //                returndays = 0.5;

        //            if ((int)firstDay.DayOfWeek == 6 & firstPeriod == Period.PM)
        //                return 0;

        //            //subtract the number of bank holidays during the time interval
        //            if (bankHolidays.Contains(firstDay.Date))
        //                return 0;

        //            return returndays;
        //        }
        //    }
        //}

    }
}