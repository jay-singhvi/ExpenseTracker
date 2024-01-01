using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.Storages;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Services.Foundations.Transactions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly ITransactionService transactionService;

        public TransactionServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.transactionService = new TransactionService(storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);


        }

        private static Transaction CreateRandomTransaction(DateTimeOffset dates) =>
            CreatePostFiller(dates: dates).Create();

        private static Transaction CreateRandomTransaction() =>
            CreatePostFiller(dates: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static Filler<Transaction> CreatePostFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Transaction>();
            filler.Setup()
                   .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
