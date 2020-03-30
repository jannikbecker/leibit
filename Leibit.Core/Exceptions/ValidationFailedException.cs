using System;

namespace Leibit.Core.Exceptions
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException()
            : base()
        {
        }

        public ValidationFailedException(string message)
            : base(message)
        {
        }
    }
}
