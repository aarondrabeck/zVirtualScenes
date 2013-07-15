using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs
{
    public static class ExceptionExtensionMethods
    {
        public static string GetInnerMostExceptionMessage(this Exception ex)
        {
            if (string.IsNullOrEmpty(ex.Message))
                return "";

            if (ex.Message.Contains("See the inner exception for details.") && ex.InnerException != null)
                return ex.InnerException.GetInnerMostExceptionMessage();
            else
                return ex.Message;
        }

    }
}
