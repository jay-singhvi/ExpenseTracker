using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Xunit.Sdk;

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

    }
}
