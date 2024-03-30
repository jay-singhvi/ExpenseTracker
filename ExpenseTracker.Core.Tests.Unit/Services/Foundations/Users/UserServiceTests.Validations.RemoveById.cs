// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

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
        public async void ShouldThrowValidationExceptionOnRemoveByIdIfUserIdIsInvalidAndLogItAsync()
        {
            // Given
            Guid randomId = Guid.Empty;
            Guid invalidUserId = randomId;

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.Id), 
                    parameterValue: invalidUserId
                    );

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(invalidUserId);

            var actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() =>
                    removeUserByIdTask.AsTask());

            // Then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowNotFoundExceptionOnRemoveByIdIfUserIsNotFoundAndLogItAsync()
        {
            // Given
            Guid randomUserId = Guid.NewGuid();
            Guid invalidUserId = randomUserId;
            User noUser = null;
            var notFoundUserException = 
                new NotFoundUserException(
                message: $"Coundn't find user with id: {invalidUserId}", 
                userId: invalidUserId
                );

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: notFoundUserException
                    );

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(invalidUserId))
                    .ReturnsAsync(noUser);

            // When
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(invalidUserId);

            var actualUserValidationException =
                    await Assert.ThrowsAsync<UserValidationException>(() =>
                        removeUserByIdTask.AsTask());

            // Then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(invalidUserId)
                    , Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(
                    It.IsAny<User>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
