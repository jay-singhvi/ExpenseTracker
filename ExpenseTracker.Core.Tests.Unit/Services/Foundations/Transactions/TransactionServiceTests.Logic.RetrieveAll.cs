// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Moq;
using System.Linq;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllTransactions()
        {
            // Given
            IQueryable<Transaction> randomTransactions = CreateRandomTransactions();
            IQueryable<Transaction> storageTransactions = randomTransactions;
            IQueryable<Transaction> expectedTransactions = randomTransactions;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactions())
                    .Returns(storageTransactions);

            // When
            IQueryable<Transaction> actualTransactions =
                this.transactionService.RetrieveAllTransactions();

            // Then
            actualTransactions.Should()
                .BeEquivalentTo(expectedTransactions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactions(),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
