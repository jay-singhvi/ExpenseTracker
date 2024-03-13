using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;

using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfTransactionIsNull()
        {
            // Given
            Transaction nullTransaction = null;

            var nullTransactionException =
                new NullTransactionException();

            var expectedTransactionValidationException =
                new TransactionValidationException(nullTransactionException);


            // When
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(nullTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionValidationException>(() =>
                addTransactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfTransactionIsInvalidAndLogItAsync(string invalidText)
        {
            // Given
            Transaction invalidTransaction = new Transaction
            {
                PaymentMode = invalidText,
                Category = invalidText,
                Description = invalidText
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

            invalidTransactionException.AddData(
                key: nameof(Transaction.Description),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.PaymentMode),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: "Date is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: "Date is required.");

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            // When
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(invalidTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionValidationException>(() =>
                addTransactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDateIsNotSameAndLogItAsync()
        {
            // Given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomNumber = GetRandomNumber();
            Transaction randomTransaction = CreateRandomTransaction(randomDateTime);
            Transaction invalidTransaction = randomTransaction;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);

            invalidTransaction.UpdatedDate =
                invalidTransaction.CreatedDate.AddDays(randomNumber);

            var invalidTransactionException =
                new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: $"Date is not same as {nameof(Transaction.CreatedDate)}");

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            // When
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.AddTransactionAsync(invalidTransaction);

            // Then
            await Assert.ThrowsAsync<TransactionValidationException>(() =>
                addTransactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExcptionOnAddIfCreatedDateIsNotRecent(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTime.AddMinutes(minutesBeforeOrAfter);

            var randomTransaction = CreateRandomTransaction(invalidDateTime);
            var invalidTransaction = randomTransaction;

            var invalidTransactionException =
                new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: "Date is not recent.");

            var expectedTransactionValidationException =
                new TransactionValidationException(invalidTransactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTime);

            // when
            ValueTask<Transaction> addPropertyTask = this.transactionService.AddTransactionAsync(invalidTransaction);

            // then
            await Assert.ThrowsAsync<TransactionValidationException>(() =>
                addPropertyTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
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
