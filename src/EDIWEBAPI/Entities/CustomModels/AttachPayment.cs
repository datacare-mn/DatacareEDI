using EDIWEBAPI.Entities.DBModel.Payment;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class AttachPayment
    {
        public REQ_PAYMENT payment { get; set; }

        public IFormFile uploadedFile { get; set; }
    }
}
