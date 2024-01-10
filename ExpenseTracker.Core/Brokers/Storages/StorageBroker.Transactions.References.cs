using ExpenseTracker.Core.Models.Transactions;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Core.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void SetTransactionReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasKey(transaction => transaction.Id);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.User)
                .WithMany(user => user.Transactions)
                .HasForeignKey(trasaction => trasaction.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
