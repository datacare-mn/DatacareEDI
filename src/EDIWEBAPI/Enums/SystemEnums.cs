using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EDIWEBAPI.Enums
{
    public class SystemEnums
    {
        public enum MasterRequestType
        {
            Product,
            ProductOrg,
            Feedback
        }

        public enum FileType
        {
            None,
            Image,
            File,
            Forbidden
        }

        public enum MessageType
        {
            None = 0,
            Order = 1,
            Return = 2,
            Payment = 3,

            NewUser = 11,
            ForgotPassword = 12, 
            OtherDevice = 13,   

            Brochure = 21,
            Marketing = 22,
            PaymentMass = 23,

            MasterNote = 31,
            MasterStatus = 32,
            MasterConfirm = 33,
            MasterRequest = 34
        }

        public enum RequestLogType
        {
            //Sent = 1,
            //Received = 2,
            //Confirmed = 3,
            //Rejected = 4,
            Created = 1,
            StatusChanged = 2,
            Edited = 5,
            NoteAdded = 6
        }
        public enum ProductImageType
        {
            Official = 1,
            Product = 2,
            Organization = 3
        }

        public enum RequestStatus
        {
            Sent = 1,
            Received = 2,
            Confirmed = 3,
            Rejected = 4
        }
        public enum MethodType
        {
            POST = 1,
            GET = 2,
            PUT = 3,
            DELETE = 4
        }

        public enum AttachFileType
        {
            SKUImage = 1,
            EmployeeProfilePic = 2,
            CompanyLogo = 3,
            BranchImage = 4,
            AttachSampleFile = 5

        }
        public  enum SystemUserTypes
        {
            SystemAdministrator = 3,
            Store = 2,
            Business = 1

        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ENABLED
        {
            Идэвхитэй = 1,
            Идэвхигүй = 0
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ISFOREIGN
        {
            Импорт = 1,
            Дотоод = 0
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ORGTYPE
        {
            Бизнес = 1,
            Дэлгүүр = 2,
            Менежмент = 3
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum NotifcationType
        {
            Захиалга  = 1,
            Буцаалт = 2,
            Мессеж = 3,
            Бараа = 4,
            ШинэХарилцагч = 5,
            Санал = 6
        }


        public enum UserProperties
        {
            UserMail,
            UserId,
            CompanyId, 
            CompanyReg,
            OrgType,
            IsHeaderCompanyUser,
            CompanyName,
            Roleid,
            Isagreement,
            IsForeign
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkuMeasure
        {
            ш = 1,
            кг = 2,
            гр = 3,
            л = 4,
            мл = 5,
            м = 6
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkuKeepUnit
        {
            хоног = 1,
            цаг = 2,
            сар = 3,
            жил = 4
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkuIsCalcVat {
            Тийм = 1,
            Үгүй = 0
        }


        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkuBalance
        {
            Бэлэн = 1,
            Дууссан = 0
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkuIsActive
        {
            Идэвхитэй = 1,
            Идэвхигүй = 0
        }

        public enum DayOfWeekMN
        {
            Ням = 0,
            Даваа = 1,
            Мягмар = 2,
            Лхагва = 3,
            Пүрэв = 4,
            Баасан = 5,
            Бямба = 6,
        }

     
    }
}
