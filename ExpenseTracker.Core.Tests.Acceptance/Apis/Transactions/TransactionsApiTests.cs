// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Tests.Acceptance.Brokers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
            Guid transactionId = Guid.NewGuid();

            var filler = new Filler<Transaction>();            

            filler.Setup()
                .OnProperty(transaction => transaction.CreatedBy).Use(transactionId)
                .OnProperty(transaction => transaction.UpdatedBy).Use(transactionId)
                .OnProperty(transaction => transaction.CreatedDate).Use(dates)
                .OnProperty(transaction => transaction.UpdatedDate).Use(dates)
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);
                //.OnProperty(transaction => transaction.User).IgnoreIt();

            return filler;
        }

        private static User CreateRandomUser()
        {
            DateTimeOffset dates = GetRandomDateTimeOffset();

            return CreateUserFiller(dates).Create();
        }

        private async ValueTask<List<Transaction>> CreateRandomPostedTransactionsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomTransactions = new List<Transaction>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomTransactions.Add(await PostRandomTransactionAsync());
            }

            return randomTransactions;
        }

        private async ValueTask<Transaction> PostRandomTransactionAsync()
        {
            Transaction randomTransaction = CreateRandomTransaction();
            await this.apiBroker.PostTransactionAsync(randomTransaction);

            return randomTransaction;
        }

        private static Transaction UpdateRandomTransaction(Transaction inputTransaction)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<Transaction>();
            
            filler.Setup()
                .OnProperty(transaction => transaction.Id).Use(inputTransaction.Id)
                .OnProperty(transaction => transaction.UserId).Use(inputTransaction.UserId)
                .OnProperty(transaction => transaction.CreatedBy).Use(inputTransaction.CreatedBy)
                .OnProperty(transaction => transaction.UpdatedBy).Use(inputTransaction.UpdatedBy)
                .OnProperty(transaction => transaction.CreatedDate).Use(inputTransaction.CreatedDate)
                .OnProperty(transaction => transaction.UpdatedDate).Use(now)
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset());

            return filler.Create();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

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
