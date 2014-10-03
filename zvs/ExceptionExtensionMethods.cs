using System;

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
