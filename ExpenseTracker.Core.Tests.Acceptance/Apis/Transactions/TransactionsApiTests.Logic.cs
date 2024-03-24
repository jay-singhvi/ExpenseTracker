// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    }
}
