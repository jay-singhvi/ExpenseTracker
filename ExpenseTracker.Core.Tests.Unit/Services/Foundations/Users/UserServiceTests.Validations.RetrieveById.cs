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

            // Then
            await Assert.ThrowsAsync<UserValidationException>(() => 
                retrieveUserById.AsTask());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), 
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserById(It.IsAny<Guid>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
