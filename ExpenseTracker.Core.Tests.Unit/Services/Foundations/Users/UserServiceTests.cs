using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.UserManagers;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Services.Foundations.Users;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        private readonly Mock<IUserManagerBroker> userManagerBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            this.userManagerBrokerMock = new Mock<IUserManagerBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userService =
                new UserService(
                userManagerBroker: this.userManagerBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static User CreateRandomUser(DateTimeOffset dates) =>
            CreateUserFiller(dates).Create();

        private static User CreateRandomUser()
        {
            DateTimeOffset dates = GetRandomDateTimeOffset();

            return CreateUserFiller(dates).Create();
        }

        private static string GetRandomPassword() =>
            new MnemonicString(1, 8, 20).GetValue();

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static string GetRandomPhoneNumber() =>
            new MnemonicString(wordCount: 1, wordMinLength: 10, wordMaxLength: 10)
            .GetValue();

        private static string GetRandomNames(NameStyle nameStyle) =>
            new RealNames(nameStyle).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        public static TheoryData InvalidMinuteCases()
        {
            int randomMoreThanMinuteFromNow = GetRandomNumber();
            int randomMoreThanMinuteBeforeNow = GetNegativeRandomNumber();

            return new TheoryData<int>
            {
                randomMoreThanMinuteFromNow,
                randomMoreThanMinuteBeforeNow
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() => new IntRange(min: 2, max: 90).GetValue();
        private static int GetNegativeRandomNumber() => -1 * GetRandomNumber();

        private static string GetRandomMessage() =>
            new MnemonicString().GetValue();

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateUserFiller(dates: GetRandomDateTimeOffset())
                    .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<User> CreateUserFiller(DateTimeOffset dates)
        {
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)
                .OnProperty(user => user.LockoutEnd).Use(dates);

            return filler;
        }
    }
}
