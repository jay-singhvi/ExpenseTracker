using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldRemoveTransactionByIdAsync()
        {
            // Given
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction storageTransaction = randomTransaction;
            Transaction expectedInputTransaction = storageTransaction;
            Transaction deletedTransaction = expectedInputTransaction;
            Transaction expectedTransaction = deletedTransaction.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(randomTransaction.Id))
                    .ReturnsAsync(storageTransaction);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteTransactionAsync(expectedInputTransaction))
                    .ReturnsAsync(deletedTransaction);

            // When
            var actualDeletedTransaction =
                await this.transactionService.RemoveTransactionByIdAsync(randomTransaction.Id);

            // Then
            actualDeletedTransaction.Should()
                .BeEquivalentTo(expectedInputTransaction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
