// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            var sqlException = GetSqlException();

            var failedTransactionStorageException =
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(failedTransactionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // When
            ValueTask<Transaction> modifyTask =
                this.transactionService.ModifyTransactionAsync(someTransaction);

            var actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(modifyTask.AsTask);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyExceptionOnModifyIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // Given
            Transaction randomTransaction = CreateRandomTransaction();
            var dbUpdateException = new DbUpdateException();

            var failedTransactionStorage =
                new FailedTransactionStorageException(dbUpdateException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(failedTransactionStorage);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(randomTransaction);

            var actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    modifyTransactionTask.AsTask);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnModifyIfdbUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedTransactionException =
                new LockedTransactionException(dbUpdateConcurrencyException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(lockedTransactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateConcurrencyException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(someTransaction);

            var actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(modifyTransactionTask.AsTask);

            // Then
            actualTransactionDependencyValidationException.Should()
                .BeEquivalentTo(expectedTransactionDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();

            Transaction foreignKeyConflictedTransaction = someTransaction;

            string expectionMessage = GetRandomMessage();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(expectionMessage);

            var invalidTransactionReference =
                new InvalidTransactionReferenceException(foreignKeyConstraintConflictException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(invalidTransactionReference);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(someTransaction);

            var actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(
                    modifyTransactionTask.AsTask);

            // Then
            actualTransactionDependencyValidationException.Should()
                .BeEquivalentTo(expectedTransactionDependencyValidationException);

            this.dateTimeBrokerMock.Verify(
                broker => broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowServiceExceptiononModifyIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            var serviceError = new Exception();

            var failedTransactionServiceException =
                new FailedTransactionServiceException(serviceError);

            var expectedTransactionServiceException =
                new TransactionServiceException(failedTransactionServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceError);

            // When
            ValueTask<Transaction> modifyTask =
                this.transactionService.ModifyTransactionAsync(someTransaction);

            var actualTransactionServiceException =
                await Assert.ThrowsAsync<TransactionServiceException>(modifyTask.AsTask);

            // Then
            actualTransactionServiceException.Should()
                .BeEquivalentTo(expectedTransactionServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
