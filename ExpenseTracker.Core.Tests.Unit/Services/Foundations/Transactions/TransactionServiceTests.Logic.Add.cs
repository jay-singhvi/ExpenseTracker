using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async Task ShouldAddTransactionAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomTransaction(dateTime);
            Transaction inputTransaction = randomTransaction;
            Transaction storageTransaction = inputTransaction;
            Transaction expectedTransaction = storageTransaction.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(dateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertTransactionAsync(inputTransaction))
                    .ReturnsAsync(storageTransaction);

            // when
            Transaction actualTransaction =
                await this.transactionService.AddTransactionAsync(inputTransaction);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(inputTransaction),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
