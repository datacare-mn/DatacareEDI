using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Helpers
{
    public static class ReturnMessageHelper
    {
        public  const string NOTFOUNDDATA = "Таны хүсэлтэнд өгөгдөл олдсонгүй!";
        public  const string SUCCESSSAVE = "Амжилттай хадгаллаа!";
        public  const string SAVEFAILURE = "Мэдээлэл хадгалахад алдаа гарлаа";
        public const string SUCCESSLOADDATA = "Мэдээллийг ажилттай татлаа";
        public const string MODELNOTVALID = "Талбарыг утгуудыг бөглөнө үү!";
        public const string USERNAMEORPASSWORDNOTVALID = "Хэрэглэгчийн нэр эсвэл нууц үг буруу байна";
        public const string MAILALREADYREGISTERED = "Мейл хаяг бүртгэгдсэн байна.";
        public const string MAILNOTREGISTERED = "Мейл бүртгэгдээгүй байна.";

    }
}
