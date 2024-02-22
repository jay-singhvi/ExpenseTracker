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
    }
}
