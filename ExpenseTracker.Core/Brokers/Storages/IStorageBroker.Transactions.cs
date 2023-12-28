using ExpenseTracker.Core.Models.Transactions;

namespace ExpenseTracker.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Transaction> InsertTransactionAsync(Transaction transaction);
        IQueryable<Transaction> SelectAllTransactions();
        ValueTask<Transaction> SelectTransactionByIdAsync(Guid transactionId);
        ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction);
        ValueTask<Transaction> DeleteTransactionAsync(Transaction transaction);
    }
}
