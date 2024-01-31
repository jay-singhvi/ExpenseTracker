using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.UserManagers;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Services.Foundations.Users;
using Moq;
using System;
using System.Linq.Expressions;
using Tynamix.ObjectFiller;
using Xeptions;

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

        private static User CreateRandomUser(DateTimeOffset dates)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomEmail(),
                PhoneNumber = GetRandomPhoneNumber(),
                FirstName = GetRandomNames(NameStyle.FirstName),
                LastName = GetRandomNames(NameStyle.LastName),
                CreatedDate = dates,
                UpdatedDate = dates
            };

            return user;
        }

        private static User CreateRandomUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomEmail(),
                PhoneNumber = GetRandomPhoneNumber(),
                FirstName = GetRandomNames(NameStyle.FirstName),
                LastName = GetRandomNames(NameStyle.LastName),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };

            return user;
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

    }
}
