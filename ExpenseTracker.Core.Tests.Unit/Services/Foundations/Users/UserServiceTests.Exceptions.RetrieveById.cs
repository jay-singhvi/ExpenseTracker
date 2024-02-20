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
        public async void ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            Guid userId = someUser.Id;

            var sqlException = GetSqlException();

            var failUserStorageException = 
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException = 
                new UserDependencyException(failUserStorageException);

            this.userManagerBrokerMock.Setup(broker => 
                broker.SelectUserById(userId))
                    .ThrowsAsync(sqlException);

            // When
            ValueTask<User> retrieveUserById = 
                this.userService.RetrieveUserByIdAsync(userId);

            var actualUserDependencyException = 
                await Assert.ThrowsAsync<UserDependencyException>(() =>
                    retrieveUserById.AsTask());

            // Then
            actualUserDependencyException.Should().BeEquivalentTo(expectedUserDependencyException);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserById(It.IsAny<Guid>()), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), 
                        Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
