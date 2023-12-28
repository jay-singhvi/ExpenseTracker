

using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService
    {
        private delegate ValueTask<Transaction> ReturningPostFunction();
        private delegate IQueryable<Transaction> ReturningPostsFunction();

        private async ValueTask<Transaction> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (SqlException sqlException)
            {
                var failedTransactionStorageException = 
                    new FailedTransactionStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);

            }
        }

        private TransactionDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var transactionDependencyException =
                new TransactionDependencyException(exception);

            this.loggingBroker.LogCritical(transactionDependencyException);

            return transactionDependencyException;
        }

    }
}
