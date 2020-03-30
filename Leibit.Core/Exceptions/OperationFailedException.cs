using System;

namespace Leibit.Core.Exceptions
{
    public class OperationFailedException : Exception
    {
        public OperationFailedException()
            : base()
        {
        }

        public OperationFailedException(string message)
            : base(message)
        {
        }
    }
}
