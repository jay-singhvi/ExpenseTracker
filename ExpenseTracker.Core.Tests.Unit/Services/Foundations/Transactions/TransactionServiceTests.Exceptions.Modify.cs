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
        public async void ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorAndLogItAsync()
        {
            // Given
            Transaction someTransaction = CreateRandomTransaction();
            var sqlException = GetSqlException();

            var failedTransactionStorageException = 
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException = 
                new TransactionDependencyException(failedTransactionStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // When
            ValueTask<Transaction> modifyTask = 
                this.transactionService.ModifyTransactionAsync(someTransaction);

            var actualTransactionDependencyException = 
                await Assert.ThrowsAsync<TransactionDependencyException>(modifyTask.AsTask);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()), 
                        Times.Never);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()), 
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
