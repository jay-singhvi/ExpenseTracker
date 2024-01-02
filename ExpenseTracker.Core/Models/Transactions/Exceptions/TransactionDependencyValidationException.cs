using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class TransactionDependencyValidationException : Xeption
    {
        public TransactionDependencyValidationException(Xeption innerException)
            : base(message: "Transaction dependency validation error occurred, please try again.", innerException)
        { }
    }
}
