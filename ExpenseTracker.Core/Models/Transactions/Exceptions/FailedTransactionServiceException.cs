// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class FailedTransactionServiceException : Xeption
    {
        public FailedTransactionServiceException(Exception innerException)
            : base(message: "Failed transaction service error occurred, please contact support.", innerException)
        { }
    }
}
