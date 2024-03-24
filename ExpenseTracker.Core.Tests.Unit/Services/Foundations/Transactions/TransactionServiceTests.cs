// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.Storages;
using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Services.Foundations.Transactions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

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
            CreateTransactionFiller(dates: dates).Create();

        private static Transaction CreateRandomTransaction() =>
            CreateTransactionFiller(dates: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static Filler<Transaction> CreateTransactionFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Transaction>();
            filler.Setup()
                   .OnType<DateTimeOffset>().Use(dates)
                   .OnProperty(transaction => transaction.User).IgnoreIt();

            return filler;
        }

        private static IQueryable<Transaction> CreateRandomTransactions() =>
            CreateTransactionFiller(dates: GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                    .AsQueryable();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int> {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomMessage() =>
            new MnemonicString().GetValue();
    }
}
