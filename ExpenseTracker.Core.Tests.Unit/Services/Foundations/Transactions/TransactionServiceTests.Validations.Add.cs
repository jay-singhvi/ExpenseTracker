using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
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
    }
}
