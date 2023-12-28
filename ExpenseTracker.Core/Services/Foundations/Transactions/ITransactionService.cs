using ExpenseTracker.Core.Models.Transactions;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public interface ITransactionService
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
    }
}
