using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs
{
    public class Result
    {
        public virtual bool HasError { get; protected set; }
        public virtual string Message { get; protected set; }

        /// <summary>
        /// Result with error
        /// </summary>
        /// <param name="message"></param>
        public Result(string message)
        {
            HasError = true;
            Message = message;
        }

        /// <summary>
        /// Result without error
        /// </summary>
        /// <param name="hasError"></param>
        /// <param name="reason"></param>
        public Result()
        {
            HasError = false;
            Message = string.Empty;
        }
    }
}