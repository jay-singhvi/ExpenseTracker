// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Transaction> Transactions { get; set; }

        public async ValueTask<Transaction> InsertTransactionAsync(Transaction transaction) =>
            await InsertAsync(transaction);

        public IQueryable<Transaction> SelectAllTransactions() =>
            SelectAll<Transaction>();

        public async ValueTask<Transaction> SelectTransactionByIdAsync(Guid transactionId) =>
            await SelectAsync<Transaction>(transactionId);

        public async ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction) =>
            await UpdateAsync(transaction);

        public async ValueTask<Transaction> DeleteTransactionAsync(Transaction transaction) =>
            await DeleteAsync(transaction);
    }
}
