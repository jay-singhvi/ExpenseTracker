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
    }
}
