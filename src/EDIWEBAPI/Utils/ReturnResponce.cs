using EDIWEBAPI.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public static class ReturnResponce
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ResponseClient GetExceptionResponce(Exception ex, bool defaultMessage = true)
        {
            var message = UsefulHelpers.GetExceptionMessage(ex);
            return new ResponseClient()
            {
                Success = false,
                Value = null,
                Message = defaultMessage ? $"Хүсэлтэнд алдаа гарлаа дахин оролдоно уу!{Environment.NewLine}{message}" : message
            };
        }

        public static ResponseClient AccessDeniedResponce()
        {
            return new ResponseClient()
            {
                Success = false,
                Value = null,
                Message = $"Уучлаарай таны хандах эрхгүй хүсэлт байна..."
            };
        }

        public static ResponseClient NotRegisteredEmail()
        {
            return new ResponseClient()
            {
                Success = false,
                Value = null,
                Message = $"И-Мейл хаяг бүртгэгдээгүй байна."
            };
        }

        private static bool IsList(object o)
        {
            return o != null 
                //&& o is IList 
                && o.GetType().IsGenericType 
                && (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))
                || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>)));
        }

        private static int RowCount(object o)
        {
            return IsList(o) ? ((o is IList) ? (o as IList).Count : 0) : 0;
        }

        public static ResponseClient ListReturnResponce(object value, string message = null)
        {
            return new ResponseClient()
            {
                Success = true,
                Value = value,
                Message = message ?? "OK",
                RowCount = RowCount(value)
            };
        }


        public static ResponseClient SaveSucessResponce()
        {
            return new ResponseClient()
            {
                Success = true,
                Message = ReturnMessageHelper.SUCCESSSAVE
            };
        }

        public static ResponseClient SaveSucessWithIdResponce(int id)
        {
            return new ResponseClient()
            {
                Success = true,
                Value = id,
                Message = ReturnMessageHelper.SUCCESSSAVE
            };
        }


        public static ResponseClient SaveFailureResponce()
        {
            return new ResponseClient()
            {
                Success = false,
                Message = ReturnMessageHelper.SAVEFAILURE
            };
        }


        public static ResponseClient SkuAlreadyRegistered(string skucd)
        {
            return new ResponseClient()
            {
                Success = false,
                Message = $"{skucd} Баркод бүртгэгдсэн байна."
            };
        }


        public static ResponseClient ModelIsNotValudResponce()
        {
            return new ResponseClient()
            {
                Success = false,
                Message = ReturnMessageHelper.MODELNOTVALID
            };
        }


        public static ResponseClient NotFoundResponce()
        {
            return new ResponseClient()
            {
                Success = false,
                Value = new List<string>(),
                Message = ReturnMessageHelper.NOTFOUNDDATA
            };
        }

        public static ResponseClient FailedRemoteServerNotConnectedResponce(string message)
        {
            return new ResponseClient()
            {
                Success = false,
                Value = new List<string>(),
                Message = ReturnMessageHelper.REMOTESERVERNOTCONNECTED + $"{Environment.NewLine} {message}"
            };
        }

        public static ResponseClient FailedMessageResponce(string message)
        {
            return new ResponseClient()
            {
                Success = false,
                Message = message
            };
        }

        public static ResponseClient SuccessMessageResponce(string message)
        {
            return new ResponseClient()
            {
                Success = true,
                Message = message
            };
        }
    }
}
