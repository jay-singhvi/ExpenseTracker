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
                broker.SelectUserByIdAsync(userId))
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
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), 
                        Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            Guid userId = someUser.Id;

            var serviceException = new Exception();

            var failedUserServiceException = 
                new FailedUserServiceException(serviceException);

            var expectedUserServiceException = 
                new UserServiceException(failedUserServiceException);

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ThrowsAsync(serviceException);

            // When
            ValueTask<User> RetrieveUserByIdTask = 
                this.userService.RetrieveUserByIdAsync(userId);

            var actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(() =>
                    RetrieveUserByIdTask.AsTask());

            // Then
            actualUserServiceException.Should().BeEquivalentTo(expectedUserServiceException);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserServiceException))), 
                        Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
