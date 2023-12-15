using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.Order
{
    public class ReciveOrder
    {
        /// <summary>
        /// БАЙГУУЛЛАГЫН РЕГИСТЕР
        /// </summary>
        public string RegNo { get; set; }
        /// <summary>
        /// БАЙГУУЛЛАГЫН НЭР
        /// </summary>
        public string Name { get; set; }
        public string ContractDesc { get; set; }
        public int ContractType { get; set; }
        public string OrderDate { get; set; }

        public string OrderNo { get; set; }

        public string ContractNo { get; set; }

        public string FileUrl { get; set; }

        public string StoreName { get; set; }

        public string MailBody { get; set; }

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
