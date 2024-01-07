using ExpenseTracker.Core.Models.Transactions;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public interface ITransactionService
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
    }
}
