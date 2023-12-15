using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Enums
{
    public static class SystemEnumTypes
    {
        public enum AttachFileType
        {
            SKUImage = 1,
            EmployeeProfilePic = 2,
            CompanyLogo = 3,
            BranchImage = 4,
            AttachSampleFile = 5

        }
        public enum SystemUserTypes
        {
           SystemAdministrator = 0,
           Store = 1,
           Business = 2

        }

        public enum UserProperties
        {
            UserMail,
            UserId,
            CompanyId
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
