using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DDay.iCal;
using SBSModel;
using SBSModel.Models;
using SBSModel.Common;

namespace ReadICSFile
{
    class Program
    {
        static void Main(string[] args)
        {
             // Load the calendar file            
            //var url = new Uri("http://www.officeholidays.com/ics/ics_country_code.php?iso=SG");
            //IICalendarCollection calendars = iCalendar.LoadFromFile(@"../../public-holidays-sg-2016.ics");

            /*var url = new Uri("http://www.calendarlabs.com/templates/ical/Singapore-Holidays.ics");
            //
            IICalendarCollection calendars = iCalendar.LoadFromUri(url);

            //
            // Get all events that occur today.
            //
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(2017, 12, 31);

            IList<Occurrence> occurrences = calendars.GetOccurrences
                (firstDay, lastDay);

            Console.WriteLine("Today's Events:");

            // Iterate through each occurrence and display information about it
            foreach (Occurrence occurrence in occurrences)
            {
                DateTime occurrenceTime = occurrence.Period.StartTime.Local;
                IRecurringComponent rc = occurrence.Source as IRecurringComponent;
                if (rc != null)
                    Console.WriteLine(rc.Summary + ": " + 
				occurrenceTime);
            }
            Console.WriteLine("End here");*/
            var lstPH = CSIUtil.readCSIFile("Singapore", 2017);
            if (lstPH.Count > 0)
            {
                var l = new LeaveService();
                foreach(var h in lstPH)
                {
                    Console.WriteLine(h.Holiday_Name + ": " + h.Holiday_Date);
                    
                    
                    var ph = new Holiday_Config();
                    ph.Name = h.Holiday_Name;
                    ph.Start_Date = DateUtil.ToDate(h.Holiday_Date);
                    ph.End_Date = DateUtil.ToDate(h.Holiday_Date);
                    ph.Record_Status = "Active";
                    ph.Create_By = "Moet";
                    ph.Create_On = DateTime.Now;
                    var r = l.InsertHoliday(ph);
                }                
            }
        }
    }
}
