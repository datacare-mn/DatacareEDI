
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class CalendarMail
    {
        public string MailAddress { get; set; }

        public string Description { get; set; }

        public string Subject { get; set; }

        public string Location { get; set; }

        public string MessageData { get; set; }

        public string FileUrl { get; set; }


    }
}
