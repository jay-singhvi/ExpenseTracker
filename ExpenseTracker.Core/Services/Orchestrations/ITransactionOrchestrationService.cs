// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Orchestrations
{
    public interface ITransactionOrchestrationService
    {
        public ValueTask<Transaction> CreateTransactionAsync(Transaction transaction);
    }
}
