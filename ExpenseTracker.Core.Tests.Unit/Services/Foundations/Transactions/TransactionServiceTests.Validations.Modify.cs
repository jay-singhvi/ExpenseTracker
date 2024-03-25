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
        public async void ShouldThrowValidationExceptionOnModifyIfTransactionIsNullAndLogItAsync()
        {
            // Given
            Transaction nullTransaction = null;

            var nullTransactionException =
                new NullTransactionException();

            var expectedTransactionValidationException =
                new TransactionValidationException(nullTransactionException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(nullTransaction);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    modifyTransactionTask.AsTask);

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
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public async void ShouldThrowValidationExceptionOnModifyIfTransactionIsInvalidAndLogItAsync(string invalidText)
        {
            // Given
            Transaction invalidTransaction = new Transaction
            {
                Category = invalidText
            };

            var invalidTransactionException =
                new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.Id),
                values: "Id is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UserId),
                values: "Id is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Category),
                values: "Text is required.");

            //invalidTransactionException.AddData(
            //    key: nameof(Transaction.PaymentMode),
            //    values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Description),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Amount),
                values: "Value is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.TransactionDate),
                values: "Date is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: "Date is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: ["Date is required.", $"Date is same as {nameof(Transaction.CreatedDate)}"]);

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(modifyTransactionTask.AsTask);

            // Then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfTransactionUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // Given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            Transaction randomTransaction =
                CreateRandomTransaction(dates: randomDateTime);

            Transaction invalidTransaction = randomTransaction;

            var invalidTransactionException = new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: $"Date is same as {nameof(Transaction.CreatedDate)}");

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(modifyTransactionTask.AsTask);

            // Then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async void ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // Given
            DateTimeOffset dateTime =
                GetRandomDateTimeOffset();

            Transaction someTransaction = CreateRandomTransaction();
            Transaction inputTransaction = someTransaction;

            inputTransaction.UpdatedDate =
                inputTransaction.UpdatedDate.AddMinutes(minutes);

            var invalidTransactionException =
                new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: "Date is not recent.");

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(inputTransaction);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(modifyTransactionTask.AsTask);

            // Then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfTransactionIsNotFoundAndLogItAsync()
        {
            // Given
            var someTransactionId = Guid.NewGuid();
            Transaction noTransaction = null;

            var notFoundTransactionException =
                new NotFoundTransactionException(someTransactionId);

            var expectedTransactionValidationException =
                new TransactionValidationException(notFoundTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ReturnsAsync(noTransaction);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.RetrieveTransactionByIdAsync(someTransactionId);

            var actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(() =>
                    modifyTransactionTask.AsTask());

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
