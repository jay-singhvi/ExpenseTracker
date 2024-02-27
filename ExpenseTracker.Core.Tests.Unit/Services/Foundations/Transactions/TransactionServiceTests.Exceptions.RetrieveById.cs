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
        public async void ShouldThrowCriticalDependencyExceptionOnretrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            Guid someTransactionId = Guid.NewGuid();
            var sqlException = GetSqlException();
            
            var failedTransactionStorageException = 
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException = 
                new TransactionDependencyException(failedTransactionStorageException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(sqlException);
            
            // When
            ValueTask<Transaction> retrieveTransactionByIdTask = 
                this.transactionService.RetrieveTransactionByIdAsync(someTransactionId);

            var actualTransactionDependencyException = 
                await Assert.ThrowsAsync<TransactionDependencyException>(retrieveTransactionByIdTask.AsTask);
            
            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectTransactionByIdAsync(It.IsAny<Guid>()), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))), 
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
