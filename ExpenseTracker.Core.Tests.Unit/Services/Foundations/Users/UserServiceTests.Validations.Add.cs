using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
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
        public async void ShouldThrowValidationExceptionOnAddIfUserIsNullAndLogItAsync()
        {
            // Given
            User invalidUser = null;
            string password = GetRandomPassword();

            var nullUserException = new NullUserException();

            var expectedUserValidationException = 
                new UserValidationException(nullUserException);

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
                broker.InsertUserAsync(It.IsAny<User>(),password), 
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
