// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRegisterUserAsyncAndLogItAsync()
        {
            // Given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset dateTime = randomDateTime;

            var randomUser = CreateRandomUser(dates: dateTime);
            var inputUser = randomUser;
            var storageUser = inputUser;
            var expectedUser = storageUser;
            string password = GetRandomPassword();
            var someEmail = GetRandomEmail();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(dateTime);

            this.userManagerBrokerMock.Setup(broker =>
                broker.InsertUserAsync(inputUser, password))
                    .ReturnsAsync(storageUser);

            // When
            User actualUser =
                await this.userService.RegisterUserAsync(inputUser, password);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            //this.dateTimeBrokerMock.Verify(broker =>
            //    broker.GetCurrentDateTimeOffset(),
            //        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(inputUser, password),
                    Times.Once);

            //this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
