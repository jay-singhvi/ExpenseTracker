using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class InvalidTransactionException : Xeption
    {
        public InvalidTransactionException() : 
            base(message: "Invalid transaction. Please correct the errors and try again.")
        { }
    }
}
