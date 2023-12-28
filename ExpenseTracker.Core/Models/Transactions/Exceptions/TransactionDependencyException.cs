using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class TransactionDependencyException : Xeption
    {
        public TransactionDependencyException(Xeption innerException)
            : base(message: "Transaction dependency error occurred, contact support.", innerException)
        {

        }
    }
}
