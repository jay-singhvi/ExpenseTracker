// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class NotFoundTransactionException : Xeption
    {
        public NotFoundTransactionException(Guid transactionId)
            : base(message: $"Couldn't find transaction with Id {transactionId}.")
        { }

        public NotFoundTransactionException(string message, Guid transactionId)
            : base(message: message)
        { }
    }
}
