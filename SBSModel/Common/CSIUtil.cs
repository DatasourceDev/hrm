using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDay.iCal;

namespace SBSModel.Common
{
    public class publicHolidays
    {
        public string Country_Name { get; set; }
        public string Holiday_Name { get; set; }
        public string Holiday_Date { get; set; }
    }
    public class CSIUtil
    {                
        public static List<publicHolidays> readCSIFile(string CountryName, int year1,Nullable<int> year2 = 0)
        {            
            // Load the calendar file                
            var url = new Uri("http://www.calendarlabs.com/templates/ical/"+ CountryName +"-Holidays.ics");
            //
            IICalendarCollection calendars = iCalendar.LoadFromUri(url);
            //
            // Get all events that occur today.
            //
            //int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year1, 1, 1);
            year2 = (year2 == 0 ? year1 : year2);
            DateTime lastDay = new DateTime(year2.Value, 12, 31);

            IList<Occurrence> occurrences = calendars.GetOccurrences
                (firstDay, lastDay);            
            // Iterate through each occurrence and display information about it
            List<publicHolidays> lstHolidays = new List<publicHolidays>();
            foreach (Occurrence occurrence in occurrences)
            {
                DateTime occurrenceTime = occurrence.Period.StartTime.Local;
                IRecurringComponent rc = occurrence.Source as IRecurringComponent;
                if (rc != null)
                {
                    Console.WriteLine(rc.Summary + ": " + occurrenceTime);
                    var ph = new publicHolidays();
                    ph.Country_Name = CountryName;
                    ph.Holiday_Name = rc.Summary;
                    ph.Holiday_Date = occurrenceTime.ToShortDateString();
                    lstHolidays.Add(ph);
                }
            }            
            return lstHolidays;
        }        
    }
}
