// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class NullTransactionException : Xeption
    {
        public NullTransactionException() : base(message: "Transaction is null.")
        { }
    }
}
