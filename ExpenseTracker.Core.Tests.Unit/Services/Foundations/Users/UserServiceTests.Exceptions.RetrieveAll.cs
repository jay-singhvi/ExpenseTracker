using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogIt()
        {
            // Given
            SqlException sqlException = GetSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Throws(sqlException);

            // Then
            Action retrieveAllUsers = () =>
                this.userService.RetrieveAllUsers();

            UserDependencyException actualUserDependencyException =
                Assert.Throws<UserDependencyException>(retrieveAllUsers);

            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectAllUsers(),
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
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogIt()
        {
            // Given
            var serviceException = new Exception();

            var failedUserStorageException =
                new FailedUserStorageException(serviceException);

            var expectedUserServiceException =
                new UserServiceException(failedUserStorageException);

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Throws(serviceException);

            // When
            Action retrieveAllUsers = () =>
                this.userService.RetrieveAllUsers();

            var actualUserServiceException =
                Assert.Throws<UserServiceException>(retrieveAllUsers);

            // Then
            actualUserServiceException.Should().BeEquivalentTo(expectedUserServiceException);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectAllUsers(),
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
