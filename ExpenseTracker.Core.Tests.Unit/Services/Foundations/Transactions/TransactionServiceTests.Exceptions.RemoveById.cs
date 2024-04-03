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
        public async void ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            Guid randomTransactionId = Guid.NewGuid();
            var sqlException = GetSqlException();

            var failTransactionStorageException =
                new FailedTransactionStorageException(
                    message: "Failed transaction storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.",
                    innerException: failTransactionStorageException);

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

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();

            var dbUpdateException = new DbUpdateException();

            var failedTransactionStorageException =
                new FailedTransactionStorageException(
                    message: "Failed transaction storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.",
                    innerException: failedTransactionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // When
            ValueTask<Transaction> removeTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionDependencyException>(() =>
                removeTransactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedTransactionDependencyException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnRemoveByIdIfReferenceErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();

            Transaction foreignKeyConflictedTransaction = someTransaction;

            string expectionMessage = GetRandomMessage();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(expectionMessage);

            var invalidTransactionReference =
                new InvalidTransactionReferenceException(
                    message: "Invalid transaction reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred, please try again.",
                    innerException: invalidTransactionReference);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // When
            ValueTask<Transaction> removeTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            var actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(
                    removeTransactionTask.AsTask);

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
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDBUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // Given
            Guid someTransactionID = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedTransactionException =
                new LockedTransactionException(
                    message: "Locked transaction record exception, please try again later.",
                    innerException: dbUpdateConcurrencyException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred, please try again.",
                    innerException: lockedTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionID))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // When
            ValueTask<Transaction> removeByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionID);

            var actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(removeByIdTask.AsTask);

            // Then
            actualTransactionDependencyValidationException.Should()
                .BeEquivalentTo(expectedTransactionDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();

        }

        [Fact]
        public async void ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            Guid someTransactionId = Guid.NewGuid();

            var serviceError = new Exception();

            var failedTransactionServiceException =
                new FailedTransactionServiceException(
                    message: "Failed transaction service error occurred, please contact support.",
                    innerException: serviceError);

            var expectedTransactionServiceException =
                new TransactionServiceException(
                    message: "Transaction service error occurred, please contact support.",
                    innerException: failedTransactionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(serviceError);

            // When
            ValueTask<Transaction> removeByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            var actualTransactionServiceException =
                await Assert.ThrowsAsync<TransactionServiceException>(
                    removeByIdTask.AsTask);

            // Then
            actualTransactionServiceException.Should()
                .BeEquivalentTo(expectedTransactionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
