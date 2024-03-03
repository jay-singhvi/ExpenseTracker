using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class LockedTransactionException : Xeption
    {
        public LockedTransactionException(Exception innerException)
            : base(message: "Locked transaction record exception, please try again later.", innerException)
        { }
    }
}
