namespace zvs
{
    public class Result
    {
        public virtual bool HasError { get; private set; }
        public virtual string Message { get; private set; }
        public static Result ReportError()
        {
            return new Result(true, string.Empty);
        }

        public static Result ReportError(string errorMessage)
        {
            return new Result(true, errorMessage);
        }

        public static Result ReportErrorFormat(string errorMessage, params object[] args)
        {
            return new Result(true, string.Format(errorMessage, args));
        }

        public static Result ReportSuccess()
        {
            return new Result(false, string.Empty);
        }

        public static Result ReportSuccess(string message)
        {
            return new Result(false, message);
        }

        public static Result ReportSuccessFormat(string message, params object[] args)
        {
            return new Result(false, string.Format(message, args));
        }

        protected Result(bool hasError, string message)
        {
            HasError = hasError;
            Message = message;
        }
    }
}