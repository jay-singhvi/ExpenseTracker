// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class AlreadyExistsTransactionException : Xeption
    {
        public AlreadyExistsTransactionException(Exception innerException)
            : base(message: "Transaction with the same id already exists.", innerException)
        { }
    }
}
