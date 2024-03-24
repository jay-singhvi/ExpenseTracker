// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Users;
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

        private static User CreateRandomUser()
        {
            DateTimeOffset dates = GetRandomDateTimeOffset();

            return CreateUserFiller(dates).Create();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static Filler<User> CreateUserFiller(DateTimeOffset dates)
        {
            var filler = new Filler<User>();

            string email = GetRandomEmail();

            filler.Setup().OnType<DateTimeOffset>().Use(dates)
                .OnProperty(user => user.Email).Use(email)
                .OnProperty(user => user.UserName).Use(email)
                .OnProperty(user => user.LockoutEnd).Use(dates);

            return filler;
        }
    }
}
