// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

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
        public async void ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // Given
            Guid invalidTransaction = Guid.Empty;

            var invalidTransactionException =
                new InvalidTransactionException(
                    message: "Invalid transaction. Please correct the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Id),
                values: "Id is required.");

            var expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occured, please try again.",
                    innerException: invalidTransactionException);

            // When
            ValueTask<Transaction> removeByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(invalidTransaction);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(removeByIdTask.AsTask);

            // Then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                        It.IsAny<Guid>()),
                            Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                        It.IsAny<Transaction>()),
                            Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnRemoveByIdIfTransactionIsNotFoundAndLogItAsync()
        {
            // Given
            var someTransactionId = Guid.NewGuid();
            Transaction noTransaction = null;

            var notFoundTransactionException =
                new NotFoundTransactionException(
                    message: $"Transaction not found with Id {someTransactionId}.",
                    transactionId: someTransactionId);

            var expectedTransactionValidationException =
                new TransactionValidationException(notFoundTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ReturnsAsync(noTransaction);

            // When
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(() =>
                    removeTransactionByIdTask.AsTask());

            // Then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
