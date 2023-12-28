using ExpenseTracker.Core.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ExpenseTracker.Core.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Transaction> Transactions { get; set; }

        public async ValueTask<Transaction> InsertPostAsync(Transaction transaction)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Transaction> transactionEntityEntry =
                await broker.Transactions.AddAsync(transaction);

            await broker.SaveChangesAsync();

            return transactionEntityEntry.Entity;
        }

        public IQueryable<Transaction> SelectAllPosts()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Transactions;
        }
        public async ValueTask<Transaction> SelectPostByIdAsync(Guid transactionId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            var retrievedTransactions = await broker.Transactions.FindAsync(transactionId);

            return retrievedTransactions;
        }
        public async ValueTask<Transaction> UpdatePostAsync(Transaction transaction)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Transaction> transactionEntityEntry =
                broker.Transactions.Update(transaction);

            await broker.SaveChangesAsync();

            return transactionEntityEntry.Entity;
        }
        public async ValueTask<Transaction> DeletePostAsync(Transaction transaction)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Transaction> transactionEntityEntry =
                broker.Transactions.Remove(transaction);

            await broker.SaveChangesAsync();

            return transactionEntityEntry.Entity;
        }
    }
}
