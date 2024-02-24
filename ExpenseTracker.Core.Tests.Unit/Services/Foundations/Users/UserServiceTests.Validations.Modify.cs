using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfUserIsNullAndLogItAsync()
        {
            // Given
            User nullUser = null;

            var nullUserException = 
                new NullUserException();

            var expectedUserValidationException = 
                new UserValidationException(nullUserException);

            // When
            ValueTask<User> modifyUserTask = 
                this.userService.ModifyUserAsync(nullUser);

            var actualUserValidationException = 
                await Assert.ThrowsAsync<UserValidationException>(() => 
                    modifyUserTask.AsTask());

            // Then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), 
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.UpdateUserAsync(It.IsAny<User>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsInvalidAndLogItAsync(string invalidText)
        {
            // Given
            User randomUser = CreateRandomUser();
            User invalidUser = randomUser;
            invalidUser.UserName = invalidText;

            var invalidUserException = 
                new InvalidUserException(
                    parameterName: nameof(User.UserName), 
                    parameterValue: invalidUser.UserName);

            var expectedUserValidationException = 
                new UserValidationException(invalidUserException);

            // When
            ValueTask<User> modifyUserTask = 
                this.userService.ModifyUserAsync(invalidUser);

            var actualUserValidationException = 
                await Assert.ThrowsAsync<UserValidationException>(() => 
                    modifyUserTask.AsTask());

            // Then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserValidationException))), 
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.UpdateUserAsync(It.IsAny<User>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfCreatedDateAndUpdatedDateIsSameAndLogItAsync()
        {
            // Given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser(randomDate);

            User inputUser = randomUser;

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.UpdatedDate),
                    parameterValue: inputUser.UpdatedDate);

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            this.userManagerBrokerMock.Setup(broker => 
                broker.SelectUserByIdAsync(inputUser.Id))
                    .Throws(invalidUserException);

            //When
            ValueTask<User> modifyUserTask =
                this.userService.RetrieveUserByIdAsync(inputUser.Id);

            var actualUserValidationException = 
                await Assert.ThrowsAsync<UserValidationException>(() => 
                    modifyUserTask.AsTask());

            //Then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(inputUser.Id), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), 
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async void ShouldThrowValidationExceptionOnModifyIfUpadtedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // Given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser(randomDate);
            User inputUser = randomUser;
            inputUser.UpdatedDate = randomDate.AddMinutes(minutes);

            var invalidUserException = new InvalidUserException(
                    parameterName: nameof(User.UpdatedDate), 
                    parameterValue: inputUser.UpdatedDate);

            var expectedUserValidationException = 
                new UserValidationException(invalidUserException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(invalidUserException);

            // When
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(inputUser);

            var actualUserValidationException = 
                await Assert.ThrowsAsync<UserValidationException>(() => 
                    modifyUserTask.AsTask());

            // Then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(), 
                    Times.Once());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), 
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.userManagerBrokerMock.Verify(broker => 
                broker.UpdateUserAsync(It.IsAny<User>()), 
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
