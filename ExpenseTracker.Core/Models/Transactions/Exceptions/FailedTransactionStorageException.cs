using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class FailedTransactionStorageException : Xeption
    {
        public FailedTransactionStorageException(Exception innerException)
            : base(message: "Failed transaction storage error occurred, contact support.", innerException)
        {

        }
    }
}
