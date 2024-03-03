using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class InvalidTransactionReferenceException : Xeption
    {
        public InvalidTransactionReferenceException(Exception innerException)
            : base(message: "Invalid transaction reference error occurred.", innerException)
        { }
    }
}
