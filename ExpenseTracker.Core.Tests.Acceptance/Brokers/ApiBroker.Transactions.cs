// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Tests.Acceptance.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string TransactionsRelativeUrl = "api/transactions";

        public async ValueTask<Transaction> PostTransactionAsync(Transaction transaction) =>
            await this.apiFactoryClient.PostContentAsync(TransactionsRelativeUrl, transaction);

        public async ValueTask<List<Transaction>> GetAllTransactionsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Transaction>>($"{TransactionsRelativeUrl}/");

        public async ValueTask<Transaction> GetTransactionByIdAsync(Guid transactionId) =>
            await this.apiFactoryClient.GetContentAsync<Transaction>($"{TransactionsRelativeUrl}/{transactionId}");

        public async ValueTask<Transaction> PutTransactionAsync(Transaction transaction) =>
            await this.apiFactoryClient.PutContentAsync<Transaction>(TransactionsRelativeUrl, transaction);

        public async ValueTask<Transaction> DeleteTransactionByIdAsync(Guid transactionId) =>
            await this.apiFactoryClient.DeleteContentAsync<Transaction>($"{TransactionsRelativeUrl}/{transactionId}");
    }
}
