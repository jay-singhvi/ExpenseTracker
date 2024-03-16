using ExpenseTracker.Core.Models.Transactions;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Orchestrations
{
    public interface ITransactionOrchestrationService
    {
        public ValueTask<Transaction> CreateTransactionAsync(Transaction transaction);
    }
}
