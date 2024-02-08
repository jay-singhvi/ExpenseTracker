using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        public async void ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            string password = GetRandomPassword();

            Exception sqlException = GetSqlException();

            var failedUserStorageException = 
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException = 
                new UserDependencyException(failedUserStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // When
            ValueTask<User> addUserTask = 
                this.userService.RegisterUserAsync(inputUser, password);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(() => addUserTask.AsTask());

            // Then
            actualUserDependencyException.Should().BeEquivalentTo(expectedUserDependencyException);

            this.dateTimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffset(), 
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.InsertUserAsync(It.IsAny<User>(),password), 
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
                new AlreadyExistsUserException(duplicateKeyException);

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(alreadyExistsUserException);

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
                new InvalidUserReferenceException(foreignKeyConstraintConflictException);

            var expectedUserDependencyValidationException = 
                new UserDependencyValidationException(invalidUserReferenceException);

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

            DbUpdateException dbUpdateException = new DbUpdateException();

            var failedUserStorageException = 
                new FailedUserStorageException(dbUpdateException);

            var expectedUserDependencyValidationException = 
                new UserDependencyValidationException(failedUserStorageException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // When
            ValueTask<User> addUserTask = 
                this.userService.RegisterUserAsync(someUser, password);

            UserDependencyValidationException actualUserDependencyValidationexception =
                await Assert.ThrowsAsync<UserDependencyValidationException>(addUserTask.AsTask);

            // then
            actualUserDependencyValidationexception.Should().BeEquivalentTo(
                expectedUserDependencyValidationException);

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
    }
}
