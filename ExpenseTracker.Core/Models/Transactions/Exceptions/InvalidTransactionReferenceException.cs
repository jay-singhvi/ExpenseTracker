// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class InvalidTransactionReferenceException : Xeption
    {
        public InvalidTransactionReferenceException(Exception innerException)
            : base(message: "Invalid transaction reference error occurred.", innerException)
        { }

        public InvalidTransactionReferenceException(string message, Exception innerException)
            : base(message: message, innerException)
        { }
    }
}
