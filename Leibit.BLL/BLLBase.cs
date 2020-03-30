using Leibit.Core.Common;
using Leibit.Core.Exceptions;

namespace Leibit.BLL
{
    public abstract class BLLBase
    {
        protected void ValidateResult<T>(OperationResult<T> Result)
        {
            if (!Result.Succeeded)
                throw new OperationFailedException(Result.Message);
        }
    }
}
