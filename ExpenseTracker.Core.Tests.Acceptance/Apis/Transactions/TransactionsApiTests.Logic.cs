// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Tests.Acceptance.Models.Transactions;
using FluentAssertions;
using Newtonsoft.Json;
using RESTFulSense.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Acceptance.Apis.Transactions
{
    public partial class TransactionsApiTests
    {
        [Fact]
        public async void ShouldPostTransactionAsync()
        {
            // Given
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction;

            // When
            Transaction actualTransaction =
                await this.apiBroker.PostTransactionAsync(inputTransaction);

            // Then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);
            await this.apiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
        }

        [Fact]
        public async Task ShouldGetAllTransactionsAsync()
        {
            // Given
            List<Transaction> randomTransactions =
                await CreateRandomPostedTransactionsAsync();

            List<Transaction> expectedTransactions = randomTransactions;

            // When
            List<Transaction> actualTransactions = 
                await this.apiBroker.GetAllTransactionsAsync();

            // Then
            foreach (Transaction expectedTransaction in expectedTransactions)
            {
                Transaction actualTransaction =
                    actualTransactions.Single(
                        transaction => transaction.Id == expectedTransaction.Id
                        );

                actualTransaction.Should().BeEquivalentTo(expectedTransaction);
                await this.apiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
            }
        }

        [Fact]
        public async Task ShouldGetTransactionByIdAsync()
        {
            // Given
            Transaction randomTransaction = await PostRandomTransactionAsync();
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = randomTransaction;

            // When
            Transaction actualTransaction = 
                await this.apiBroker.GetTransactionByIdAsync(inputTransaction.Id);

            // Then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);
            await this.apiBroker.DeleteTransactionByIdAsync(inputTransaction.Id);
        }

        [Fact]
        public async Task ShouldPutTransactionAsync()
        {
            // Given
            Transaction randomTransaction = await PostRandomTransactionAsync();
            Transaction modifiedTransaction = UpdateRandomTransaction(randomTransaction);

            // When
            await this.apiBroker.PutTransactionAsync(modifiedTransaction);

            Transaction actualTransaction = 
                await this.apiBroker.GetTransactionByIdAsync(randomTransaction.Id);

            // Then
            actualTransaction.Should().BeEquivalentTo(modifiedTransaction);
            await this.apiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
        }

        [Fact]
        public async Task ShouldDeleteTransactionAsync()
        {
            // Given
            Transaction randomTransaction = await PostRandomTransactionAsync();
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction;

            // When
            Transaction deletedTransaction = 
                await this.apiBroker.DeleteTransactionByIdAsync(inputTransaction.Id);

            ValueTask<Transaction> getTransactionByIdTask = 
                this.apiBroker.GetTransactionByIdAsync(inputTransaction.Id);

            // Then
            deletedTransaction.Should().BeEquivalentTo(expectedTransaction);
            await Assert.ThrowsAsync<HttpResponseNotFoundException>(() => 
                getTransactionByIdTask.AsTask());
        }
    }
}
