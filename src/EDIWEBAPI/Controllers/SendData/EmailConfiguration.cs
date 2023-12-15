using System;

namespace EDIWEBAPI.Controllers.SendData
{
    public class EmailConfiguration
    {
        public static readonly int SMTP_PORT = 25;

        public static readonly string URTO_SMTP_ADDRESS = "mail.urto.mn";
        public static readonly string URTO_ADDRESS_NAME = "Бизнес өртөө";
        public static readonly string URTO_OPERATOR_MAIL = "operator@urto.mn";
        public static readonly string URTO_OPERATOR_PASSWORD = "3G36XDAat5!^";

        public static readonly string STORE_SMTP_ADDRESS = "mail.e-mart.mn";
        public static readonly string STORE_ADDRESS_NAME = "Emart order information";
        public static readonly string STORE_ORDER_MAIL = "order@e-mart.mn";
        public static readonly string STORE_ORDER_PASSWORD = "B5yYbcfu";
    }
}
