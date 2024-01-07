using ExpenseTracker.Core.Models.Users;
using System;

namespace ExpenseTracker.Core.Models.Transactions
{
    public class Transaction : IAuditable
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string PaymentMode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public User User { get; set; }
    }
}
