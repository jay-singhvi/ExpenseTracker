// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class TransactionDependencyValidationException : Xeption
    {
        public TransactionDependencyValidationException(Xeption innerException)
            : base(message: "Transaction dependency validation error occurred, please try again.", innerException)
        { }

        public TransactionDependencyValidationException(string message, Xeption innerException)
            : base(message: message, innerException)
        { }
    }
}
