using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Newtonsoft.Json;
using System;
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
            User randomUser = CreateRandomUser();
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction;

            // When
            await this.apiBroker.PostUserAsync(randomUser);

            await this.apiBroker.LoginUserAsync(randomUser);

            //FindUser
            Guid userId = this.apiBroker.

            //inputTransaction.UserId = storageUser.Id;

            //var json = JsonConvert.SerializeObject(inputTransaction);

            await this.apiBroker.PostTransactionAsync(inputTransaction);

            // Then

            var actualTransaction = await this.apiBroker.GetTransactionByIdAsync(inputTransaction.Id);

            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            await this.apiBroker.DeleteTransactionByIdAsync(inputTransaction.Id);
        }
    }
}
