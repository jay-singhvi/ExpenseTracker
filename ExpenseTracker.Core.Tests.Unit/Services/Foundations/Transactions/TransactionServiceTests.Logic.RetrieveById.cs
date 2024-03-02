using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldRetrieveTransactionById()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            Transaction inputTransaction = someTransaction;
            Transaction storageTransaction = inputTransaction;
            Transaction expectedTransaction = inputTransaction;

            Guid transactionId = inputTransaction.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(transactionId))
                    .ReturnsAsync(storageTransaction);

            // When
            ValueTask<Transaction> retrieveTransactionByIdTask =
                this.transactionService.RetrieveTransactionByIdAsync(transactionId);

            var actualTransaction = await retrieveTransactionByIdTask.AsTask();
            // Then
            actualTransaction.Should()
                .BeEquivalentTo(expectedTransaction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(transactionId),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
