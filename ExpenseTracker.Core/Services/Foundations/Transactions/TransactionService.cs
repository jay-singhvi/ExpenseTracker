using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.Storages;
using ExpenseTracker.Core.Models.Transactions;

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
                this.dateTimeBroker.GetCurrentDateTimeOffset();
                return await this.storageBroker.InsertTransactionAsync(transaction);
            });
    }
}
