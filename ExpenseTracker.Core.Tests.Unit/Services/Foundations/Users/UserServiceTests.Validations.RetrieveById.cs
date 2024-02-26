using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
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
        public async void ShouldThrowValidationExceptionOnRetrieveByIdIfUserIsInvalidAndLogItAsync()
        {
            // Given
            Guid invalidUserId = Guid.Empty;

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.Id),
                    parameterValue: invalidUserId);

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // When
            ValueTask<User> retrieveUserById =
                this.userService.RetrieveUserByIdAsync(invalidUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() =>
                    retrieveUserById.AsTask());

            // Then

            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnRetrieveByIdIfUserNotFoundAndLogItAsync()
        {
            // Given
            Guid invalidUserId = Guid.NewGuid();
            User noUser = null;

            var notFoundException =
                new NotFoundUserException(invalidUserId);

            var expectedUserValidationException =
                new UserValidationException(notFoundException);

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(invalidUserId))
                    .ReturnsAsync(noUser);

            // When
            ValueTask<User> retrieveUserById =
                this.userService.RetrieveUserByIdAsync(invalidUserId);

            var actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() =>
                    retrieveUserById.AsTask());

            // Then

            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
