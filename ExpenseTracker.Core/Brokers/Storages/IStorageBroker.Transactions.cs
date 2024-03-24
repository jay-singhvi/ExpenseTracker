// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using System;
using System.Linq;
using System.Threading.Tasks;

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
