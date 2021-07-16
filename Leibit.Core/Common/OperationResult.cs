using System;
using System.Collections.Concurrent;

namespace Leibit.Core.Common
{
    public class OperationResult<TResult>
    {

        public OperationResult()
        {
            Messages = new ConcurrentBag<string>();
        }

        public bool Succeeded { get; set; }
        public TResult Result { get; set; }
        public ConcurrentBag<string> Messages { get; set; }

        public string Message
        {
            get
            {
                return String.Join(Environment.NewLine, Messages);
            }
            set
            {
                Messages.Add(value);
            }
        }

        public static OperationResult<TResult> Ok(TResult result)
        {
            var res = new OperationResult<TResult>();
            res.Result = result;
            res.Succeeded = true;
            return res;
        }

        public static OperationResult<TResult> Fail(string message)
        {
            var res = new OperationResult<TResult>();
            res.Message = message;
            res.Result = default;
            res.Succeeded = false;
            return res;
        }

    }
}
