using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldRetrieveAllTransactions()
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
