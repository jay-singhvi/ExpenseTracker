using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var invalidTransactionException = new InvalidTransactionException();

            invalidTransactionException.AddData(
                key: nameof(Transaction.Id), 
                values: "Id is required.");

            var expectedTransactionValidationException = 
                new TransactionValidationException(invalidTransactionException);

            // When
            ValueTask<Transaction> removeByIdTask = 
                this.transactionService.RemoveTransactionByIdAsync(invalidTransaction);

            var actualTransactionValidationException = 
                await Assert.ThrowsAsync<InvalidTransactionException>(removeByIdTask.AsTask);

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
    }
}
