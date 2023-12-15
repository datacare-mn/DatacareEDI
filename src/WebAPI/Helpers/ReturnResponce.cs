using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebAPI.Helpers
{
    public static class ReturnResponce
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ResponseClient GetExceptionResponce(Exception ex)
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Value = ex.ToString();
            rs.Message = $"Мэдээлэл хадгалахад алдаа гарлаа дахин оролдоно уу!{Environment.NewLine}{ex.Message}";
            return rs;
        }

        public static ResponseClient ListReturnResponce(object value)
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = true;
            rs.Value = value;
            rs.Message = "Ok";
            return rs;
        }


        public static ResponseClient SaveSucessResponce()
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = true;
            rs.Message = ReturnMessageHelper.SUCCESSSAVE;
            return rs;
        }


        public static ResponseClient SaveFailureResponce()
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Message = ReturnMessageHelper.SAVEFAILURE;
            return rs;
        }


        public static ResponseClient SkuAlreadyRegistered(string skucd)
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Message = $"{skucd} Баркод бүртгэгдсэн байна.";
            return rs;
        }


        public static ResponseClient ModelIsNotValudResponce()
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Message = ReturnMessageHelper.MODELNOTVALID;
            return rs;
        }


        public static ResponseClient NotFoundResponce()
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Message = ReturnMessageHelper.NOTFOUNDDATA;
            return rs;
        }

        public static ResponseClient FailedMessageResponce(string message)
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = false;
            rs.Message = message;
            return rs;
        }

        public static ResponseClient SuccessMessageResponce(string message)
        {
            ResponseClient rs = new ResponseClient();
            rs.Success = true;
            rs.Message = message;
            return rs;
        }









    }
}
