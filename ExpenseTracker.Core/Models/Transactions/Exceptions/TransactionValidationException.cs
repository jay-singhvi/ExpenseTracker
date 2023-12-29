using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class TransactionValidationException : Xeption
    {
        public TransactionValidationException(Xeption innerException) :
            base(message: "Transaction validation error occured, please try again.", innerException)
        { }
    }
}
