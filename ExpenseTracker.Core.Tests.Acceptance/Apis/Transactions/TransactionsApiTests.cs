using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Tests.Acceptance.Brokers;
using System;
using System.Runtime.CompilerServices;
using Tynamix.ObjectFiller;
using Xunit;

namespace ExpenseTracker.Core.Tests.Acceptance.Apis.Transactions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class TransactionsApiTests
    {
        private readonly ApiBroker apiBroker;

        public TransactionsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static Transaction CreateRandomTransaction() =>
            CreateTransactionFiller(dates: DateTimeOffset.UtcNow).Create();
        private static Transaction CreateRandomTransaction(DateTimeOffset dates) =>
            CreateTransactionFiller(dates: dates).Create();

        private static Filler<Transaction> CreateTransactionFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Transaction>();

            filler.Setup().OnType<DateTimeOffset>().Use(dates)
                .OnProperty(transaction => transaction.User).IgnoreIt();

            return filler;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();



    }
}
