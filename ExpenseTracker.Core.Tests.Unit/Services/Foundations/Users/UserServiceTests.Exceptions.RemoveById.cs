// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async void ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlExceptionErrorOccursAndLogItAsync()
        {
            // Given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;
            var sqlException = GetSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.", 
                    innerException: sqlException
                    );

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.", 
                    innerException: failedUserStorageException
                    );

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ThrowsAsync(sqlException);

            // When
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(inputUserId);

            var actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(() =>
                    removeUserByIdTask.AsTask());

            // Then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserDependencyException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDBUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;

            Guid userId = inputUser.Id;

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(
                    message: "Locked user record exception, please try again.", 
                    innerException: dbUpdateConcurrencyException
                    );

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.", 
                    innerException: lockedUserException
                    );

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // When
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(userId);

            var actualUserDependencyValidation =
                await Assert.ThrowsAsync<UserDependencyValidationException>(() =>
                    removeUserByIdTask.AsTask());

            // Then
            actualUserDependencyValidation.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            Guid userId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserServiceException =
                new FailedUserServiceException(
                    message: "Failed user service error occurred, please contact support.", 
                    innerException: serviceException
                    );

            var expectedUserServiceException =
                new UserServiceException(
                    message: "Profile service error occurred, contact support.", 
                    innerException: failedUserServiceException
                    );

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ThrowsAsync(serviceException);

            // When
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(userId);

            var actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(() =>
                    removeUserByIdTask.AsTask());
            // Then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserServiceException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
