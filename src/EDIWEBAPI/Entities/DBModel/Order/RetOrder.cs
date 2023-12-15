using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.Order
{
    public class RetOrder
    {
        public string OrderDate { get; set; }

        public string OrderNo { get; set; }

        public string ContractNo { get; set; }

        public string FileUrl { get; set; }

        public string StoreName { get; set; }

        public string MailBody { get; set; }

        public string RetIndex { get; set; }

        /// <summary>
        /// Илгээх имейл хаяг
        /// </summary>
        public string SendMail { get; set; }
        /// <summary>
        /// Мессеж илгээх дугаар
        /// </summary>
        public string SMSMobile { get; set; }
    }
}
