using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService
    {
        public void ValidateTransactionOnAdd(Transaction transaction)
        {
            ValidateTransactionIsNotNull(transaction);
        }

        private static void ValidateTransactionIsNotNull(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new NullTransactionException();
            }
        }
    }
}
