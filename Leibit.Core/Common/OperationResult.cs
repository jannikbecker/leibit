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

    }
}
