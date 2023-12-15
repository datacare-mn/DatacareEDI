using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class PaymentDeleteModel
    {
        public string ContractNo { get; set; }
        public string Date { get; set; }
        public string Reason { get; set; }
    }
}
