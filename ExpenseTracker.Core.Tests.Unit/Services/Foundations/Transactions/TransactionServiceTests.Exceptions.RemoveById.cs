using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
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
        public async void ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            Guid randomTransactionId = Guid.NewGuid();
            var sqlException = GetSqlException();

            var failTransactionStorageException = 
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException = 
                new TransactionDependencyException(failTransactionStorageException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectTransactionByIdAsync(randomTransactionId))
                    .ThrowsAsync(sqlException);

            // When
            ValueTask<Transaction> removeTransactionByIdTask = 
                this.transactionService.RemoveTransactionByIdAsync(randomTransactionId);

            var actualTransactionDependencyException = 
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    removeTransactionByIdTask.AsTask);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))), 
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
