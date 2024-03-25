// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions.Exceptions;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionErrorOccursAndLogItAsync()
        {
            // Given
            var sqlException = GetSqlException();

            var failedTransactionStorageException =
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(failedTransactionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactions())
                    .Throws(sqlException);

            // When
            Action retrieveAllTransactions = () =>
                this.transactionService.RetrieveAllTransactions();

            var actualTransactionDependencyException =
                Assert.Throws<TransactionDependencyException>(retrieveAllTransactions);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactions(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            var serviceException = new Exception();

            var failedTransactionServiceException =
                new FailedTransactionServiceException(serviceException);

            var expectedTransactionServiceException =
                new TransactionServiceException(failedTransactionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactions())
                    .Throws(serviceException);

            // When
            Action retrieveAllTransactionsAction = () =>
            this.transactionService.RetrieveAllTransactions();

            var actualTransactionServiceException =
                Assert.Throws<TransactionServiceException>(retrieveAllTransactionsAction);

            // Then
            actualTransactionServiceException.Should()
                .BeEquivalentTo(expectedTransactionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactions(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
