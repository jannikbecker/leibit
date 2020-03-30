using Leibit.Core.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Leibit.Tests.Utils
{
    internal static class DefaultChecks
    {
        internal static void IsOperationSucceeded<T>(OperationResult<T> result)
        {
            IsOperationSucceeded(result, String.Format("Operation did not succeed, Message: {0}", result.Message));
        }

        internal static void IsOperationSucceeded<T>(OperationResult<T> result, string message)
        {
            if (!result.Succeeded)
                Assert.Fail(message);
        }

        internal static void IsOperationNotSucceeded<T>(OperationResult<T> result)
        {
            IsOperationNotSucceeded(result, "Operation did succeed, but was expected to fail.");
        }

        internal static void IsOperationNotSucceeded<T>(OperationResult<T> result, string message)
        {
            if (result.Succeeded)
                Assert.Fail(message);
        }

        internal static void HasResultValue<T>(OperationResult<T> result)
        {
            HasResultValue(result, "The result value is null.");
        }

        internal static void HasResultValue<T>(OperationResult<T> result, string message)
        {
            if (result.Result == null)
                Assert.Fail(message);
        }
    }
}
