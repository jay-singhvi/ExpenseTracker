using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class NotFoundTransactionException : Xeption
    {
        public NotFoundTransactionException(Guid transactionId)
            : base(message: $"Couldn't find transaction with Id {transactionId}.")
        { }
    }
}
