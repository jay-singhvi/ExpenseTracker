using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService
    {
        private delegate ValueTask<Transaction> ReturningTransactionFunction();
        private delegate IQueryable<Transaction> ReturningTransactionsFunction();

        private async ValueTask<Transaction> TryCatch(ReturningTransactionFunction returningTransactionFunction)
        {
            try
            {
                return await returningTransactionFunction();
            }
            catch (NullTransactionException nullTransactionException)
            {
                throw CreateAndLogValidationException(nullTransactionException);
            }
            catch (InvalidTransactionException invalidTransactionException)
            {
                throw CreateAndLogValidationException(invalidTransactionException);
            }
            catch (NotFoundTransactionException notFoundTransactionException)
            {
                throw CreateAndLogValidationException(notFoundTransactionException);
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
            catch (Exception exception)
            {
                var failedTransactionServiceException =
                    new FailedTransactionServiceException(exception);

                throw CreateAndLogServiceException(failedTransactionServiceException);
            }

        }


        private IQueryable<Transaction> TryCatch(ReturningTransactionsFunction returningTransactionFunctions)
        {
            try
            {
                return returningTransactionFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedTransactionStorageException =
                    new FailedTransactionStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);
            }
            catch (Exception serviceException)
            {
                var failedTransactionServiceException =
                    new FailedTransactionServiceException(serviceException);

                throw CreateAndLogServiceException(failedTransactionServiceException);
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

        private TransactionServiceException CreateAndLogServiceException(Xeption exception)
        {
            var transactionServiceException =
                new TransactionServiceException(exception);

            this.loggingBroker.LogError(transactionServiceException);

            throw transactionServiceException;
        }

    }
}
