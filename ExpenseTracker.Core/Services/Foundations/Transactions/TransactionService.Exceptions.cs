

using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            catch (NullTransactionException nullTransactionException)
            {
                throw CreateAndLogValidationException(nullTransactionException);
            }
            catch (InvalidTransactionException invalidTransactionException)
            {
                throw CreateAndLogValidationException(invalidTransactionException);
            }
            catch (SqlException sqlException)
            {
                var failedTransactionStorageException =
                    new FailedTransactionStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsTransaction =
                    new AlreadyExistsTransactionException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsTransaction);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedTransactionStorageException =
                    new FailedTransactionStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedTransactionStorageException);
            }

        }

        private TransactionValidationException CreateAndLogValidationException(Xeption exception)
        {
            var transactionValidationException =
                new TransactionValidationException(exception);

            this.loggingBroker.LogError(transactionValidationException);

            return transactionValidationException;
        }

        private TransactionDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var transactionDependencyException =
                new TransactionDependencyException(exception);

            this.loggingBroker.LogCritical(transactionDependencyException);

            return transactionDependencyException;
        }

        private TransactionDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var transactionDependencyValidationException =
                new TransactionDependencyValidationException(exception);

            this.loggingBroker.LogError(transactionDependencyValidationException);

            return transactionDependencyValidationException;
        }

        private TransactionDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var transactionDependencyException =
                new TransactionDependencyException(exception);

            this.loggingBroker.LogError(transactionDependencyException);

            throw transactionDependencyException;
        }

    }
}
