using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.Storages;
using ExpenseTracker.Core.Models.Transactions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public TransactionService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Transaction> AddTransactionAsync(Transaction transaction) =>
            TryCatch(async () =>
            {
                ValidateTransactionOnAdd(transaction);
                return await this.storageBroker.InsertTransactionAsync(transaction);
            });

        public IQueryable<Transaction> RetrieveAllTransactions() =>
            TryCatch(() =>
            {
                return this.storageBroker.SelectAllTransactions();
            });

        public ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId) => 
            TryCatch(async () => {
                ValidateTransactionId(transactionId);

                var maybeTransaction = 
                    await this.storageBroker.SelectTransactionByIdAsync(transactionId);

                ValidateStorageTransaction(maybeTransaction, transactionId);

                return maybeTransaction;
            });

        public ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction) =>
            TryCatch(async () => {
                ValidateTransactionOnModify(transaction);

                Transaction maybeTransaction =
                    await this.storageBroker.SelectTransactionByIdAsync(transaction.Id);

                return await this.storageBroker.UpdateTransactionAsync(maybeTransaction);
            });

        public ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId) =>
            TryCatch(async () => { 
                ValidateTransactionId(transactionId);

                var maybeTransaction =
                    await this.storageBroker.SelectTransactionByIdAsync(transactionId);

                return await this.storageBroker.DeleteTransactionAsync(maybeTransaction);
            });
    }
}
