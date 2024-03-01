using ExpenseTracker.Core.Models.Transactions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public interface ITransactionService
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
        IQueryable<Transaction> RetrieveAllTransactions();
        ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId);
        ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction);
        ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId);
    }
}
