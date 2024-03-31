// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using EFxceptions.Models.Exceptions;
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
        public async void ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            string password = GetRandomPassword();

            Exception sqlException = GetSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed transaction storage error occurred, contact support.",
                    innerException: sqlException
                    );

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedUserStorageException
                    );

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(() =>
                    addUserTask.AsTask());

            // Then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnAddIfUserAlreadyExistsAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            string password = GetRandomPassword();

            string exceptionMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(exceptionMessage);

            var alreadyExistsUserException =
                new AlreadyExistsUserException(
                    message: "User with same Id already exists.",
                    innerException: duplicateKeyException
                    );

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: alreadyExistsUserException
                    );

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            UserDependencyValidationException actualUserDependencyValidationException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(() =>
                    addUserTask.AsTask());

            // Then
            actualUserDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    actualUserDependencyValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            string password = GetRandomPassword();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidUserReferenceException =
                new InvalidUserReferenceException(
                    message: "Invalid user reference error occurred.",
                    innerException: foreignKeyConstraintConflictException
                    );

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: invalidUserReferenceException
                    );

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(someUser, password);

            var actualUserDependencyValidationException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(
                    addUserTask.AsTask);

            // Then
            actualUserDependencyValidationException.Should().BeEquivalentTo(
                expectedUserDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();


        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            string password = GetRandomPassword();

            DbUpdateException dbUpdateException =
                new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: dbUpdateException
                    );

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: failedUserStorageException
                    );

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(someUser, password);

            UserDependencyValidationException actualUserDependencyValidationexception =
                await Assert.ThrowsAsync<UserDependencyValidationException>(addUserTask.AsTask);

            // then
            actualUserDependencyValidationexception.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            string password = GetRandomPassword();

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(someUser, password);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(addUserTask.AsTask);

            // Then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserServiceException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
