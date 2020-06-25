using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDay.iCal;

namespace Lib
{
    public class Class1
    {
         // Load the calendar file            
            //var url = new Uri("http://www.officeholidays.com/ics/ics_country_code.php?iso=SG");
            //IICalendarCollection calendars = iCalendar.LoadFromFile(@"../../public-holidays-sg-2016.ics");

            var url = new Uri("http://www.calendarlabs.com/templates/ical/Singapore-Holidays.ics");
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
            Console.WriteLine("End here");
    }
}
