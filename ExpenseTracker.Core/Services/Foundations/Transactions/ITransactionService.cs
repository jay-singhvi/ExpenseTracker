using ExpenseTracker.Core.Models.Transactions;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public interface ITransactionService
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
        IQueryable<Transaction> RetrieveAllTransactions();
    }
}
