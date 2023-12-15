using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public class Appointment
    {
        private StreamWriter writer = null;
        public Appointment()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string GetFormatedDate(DateTime date)
        {
            string YY = date.Year.ToString();
            string MM = string.Empty;
            string DD = string.Empty;
            if (date.Month < 10) MM = "0" + date.Month.ToString();
            else MM = date.Month.ToString();
            if (date.Day < 10) DD = "0" + date.Day.ToString();
            else DD = date.Day.ToString();
            return YY + MM + DD;
        }
        public string GetFormattedTime(string time)
        {
            string[] times = time.Split(':');
            string HH = string.Empty;
            string MM = string.Empty;
            if (Convert.ToInt32(times[0]) < 10) HH = "0" + times[0];
            else HH = times[0];
            if (Convert.ToInt32(times[1]) < 10) MM = "0" + times[0];
            else MM = times[1];
            return HH + MM + "00Z";

        }
        public string MakeDayEvent(string subject, string location, DateTime startDate, DateTime endDate)
        {
            string filePath = string.Empty;
            string path = Path.Combine(@"\iCal\");
            filePath = path + subject + ".ics";
            writer = new StreamWriter(filePath);
            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("VERSION:2.0");
            writer.WriteLine("PRODID:-//hacksw/handcal//NONSGML v1.0//EN");
            writer.WriteLine("BEGIN:VEVENT");


            string startDay = "VALUE=DATE:" + GetFormatedDate(startDate);
            string endDay = "VALUE=DATE:" + GetFormatedDate(endDate);

            writer.WriteLine("DTSTART;" + startDay);
            writer.WriteLine("DTEND;" + endDay);
            writer.WriteLine("SUMMARY:" + subject);
            writer.WriteLine("LOCATION:" + location);
            writer.WriteLine("END:VEVENT");
            writer.WriteLine("END:VCALENDAR");
            writer.Close();

            return filePath;
        }
        public string MakeHourEvent(string subject, string location, DateTime date, string startTime, string endTime)
        {
            string filePath = string.Empty;
            
            string path = Path.Combine(@"\iCal\"); 
            filePath = path + subject + ".ics";
            writer = new StreamWriter(filePath);
            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("VERSION:2.0");
            writer.WriteLine("PRODID:-//hacksw/handcal//NONSGML v1.0//EN");
            writer.WriteLine("BEGIN:VEVENT");

            string startDateTime = GetFormatedDate(date) + "T" + GetFormattedTime(startTime);
            string endDateTime = GetFormatedDate(date) + "T" + GetFormattedTime(endTime);

            writer.WriteLine("DTSTART:" + startDateTime);
            writer.WriteLine("DTEND:" + endDateTime);
            writer.WriteLine("SUMMARY:" + subject);
            writer.WriteLine("LOCATION:" + location);
            writer.WriteLine("END:VEVENT");
            writer.WriteLine("END:VCALENDAR");
            writer.Close();

            return filePath;
        }
    }
}
