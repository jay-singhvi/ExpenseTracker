using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
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
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(failedTransactionStorageException);

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
    }
}
