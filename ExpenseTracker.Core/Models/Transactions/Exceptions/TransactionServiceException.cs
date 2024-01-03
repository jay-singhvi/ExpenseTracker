using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class TransactionServiceException : Xeption
    {
        public TransactionServiceException(Xeption innerException)
            : base(message: "Transaction service error occurred, please contact support.", innerException)
        { }
    }
}
