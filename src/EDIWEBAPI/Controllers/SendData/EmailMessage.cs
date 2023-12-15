using System;
using System.Collections.Generic;
using System.Linq;

namespace EDIWEBAPI.Controllers.SendData
{
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public string UserEmail { get; set; }
        public byte Priority { get; set; }

        public string CC { get; set; }
        public string Attachment { get; set; }
        public int StoreId { get; set; }
        public EDIWEBAPI.Enums.SystemEnums.MessageType Type { get; set; }

    }
}
