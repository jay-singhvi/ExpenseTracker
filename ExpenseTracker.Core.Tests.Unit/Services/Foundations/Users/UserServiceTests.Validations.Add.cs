// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfUserIsNullAndLogItAsync()
        {
            // Given
            User invalidUser = null;
            string password = GetRandomPassword();

            var nullUserException = new NullUserException(message: "User is Null.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: nullUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(invalidUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() =>
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfIdIsInvalidAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            inputUser.Id = default;
            string password = GetRandomPassword();

            var invalidUserException = new InvalidUserException(
                parameterName: nameof(inputUser.Id),
                parameterValue: inputUser.Id);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() =>
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserNameIsInvalidAndLogItAsync(string invalidUserUserName)
        {
            // Given
            User randomUser = CreateRandomUser();
            User invalidUser = randomUser;
            invalidUser.UserName = invalidUserUserName;
            string password = GetRandomPassword();

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.UserName),
                    parameterValue: invalidUser.UserName);

            var expectedUserValidationException =
            new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(invalidUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() =>
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnAddIfFirstNameIsInvalidAndLogItAsync(string invalidUserFirstName)
        {
            // Given
            User randomUser = CreateRandomUser();
            User invalidUser = randomUser;
            invalidUser.FirstName = invalidUserFirstName;
            string password = GetRandomPassword();

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.FirstName),
                    parameterValue: invalidUser.FirstName);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(invalidUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(
                () => addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(
                    It.IsAny<User>(), password),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnAddIfLastNameIsInvalidAndLogItAsync(string invalidUserLastName)
        {
            // Given
            User randomUser = CreateRandomUser();
            User invalidUser = randomUser;
            invalidUser.LastName = invalidUserLastName;
            string password = GetRandomPassword();

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.LastName),
                    parameterValue: invalidUser.LastName);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(invalidUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() => 
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(
                    It.IsAny<User>(), password),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfCreatedDateIsInvalidAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            inputUser.CreatedDate = default;
            string password = GetRandomPassword();

            var invalidUserException = new InvalidUserException(
                parameterName: nameof(User.CreatedDate),
                parameterValue: inputUser.CreatedDate);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(
                () => addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedUserValidationException))),
                Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfUpdatedDateIsInvalidAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            inputUser.UpdatedDate = default;
            string password = GetRandomPassword();

            var invalidUserException = new InvalidUserException(
                parameterName: nameof(User.UpdatedDate),
                parameterValue: inputUser.UpdatedDate);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() => 
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedUserValidationException))),
                Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            inputUser.UpdatedDate = GetRandomDateTimeOffset();
            string password = GetRandomPassword();

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.UpdatedDate),
                    parameterValue: inputUser.UpdatedDate);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() => 
                addUserTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>(), password),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // Given
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser(dates: dateTime);
            User inputUser = randomUser;
            inputUser.CreatedDate = dateTime.AddMinutes(minutes);
            inputUser.UpdatedDate = inputUser.CreatedDate;
            string password = GetRandomPassword();

            var invalidUserException =
                new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: inputUser.CreatedDate);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User Validation error occurred, please try again.", 
                    innerException: invalidUserException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(dateTime);

            // When
            ValueTask<User> addUserTask =
                this.userService.RegisterUserAsync(inputUser, password);

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() =>
                addUserTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once());

            this.userManagerBrokerMock.Verify(broker =>
                broker.InsertUserAsync(
                    It.IsAny<User>(), password),
                        Times.Never());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
