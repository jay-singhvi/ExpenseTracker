// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurrsAndLogItAsync()
        {
            // given
            var someTransaction = CreateRandomTransaction();
            SqlException sqlException = GetSqlException();

            var failedTransactionStorageException =
                new FailedTransactionStorageException(
                    message: "Failed transaction storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.", 
                    innerException: failedTransactionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Throws(sqlException);

            // when
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            // then
            await Assert.ThrowsAsync<TransactionDependencyException>(() =>
                addTransactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
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
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionDependencyException>(() =>
                addTransactionTask.AsTask());

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
        public async void ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
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
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            var actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(
                    addTransactionTask.AsTask);

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
        public async void ShouldThrowDependencyValidationExceptionOnAddIfdbUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedTransactionException =
                new LockedTransactionException(dbUpdateConcurrencyException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred, please try again.",
                    innerException: lockedTransactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateConcurrencyException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

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
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfTransactionAlreadyExistsAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            string someMessage = GetRandomMessage();

            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsTransactionException =
                new AlreadyExistsTransactionException(
                    message: "Transaction with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred, please try again.",
                    innerException: alreadyExistsTransactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // When
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionDependencyValidationException>(() =>
                addTransactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedTransactionDependencyValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertTransactionAsync(someTransaction),
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();

        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            var serviceException = new Exception();

            var failedTransactionServiceException =
                new FailedTransactionServiceException(
                    message: "Failed transaction service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedTransactionServiceException =
                new TransactionServiceException(
                    message: "Transaction service error occurred, please contact support.",
                    innerException: failedTransactionServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // When
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(someTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionServiceException>(() =>
                addTransactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
