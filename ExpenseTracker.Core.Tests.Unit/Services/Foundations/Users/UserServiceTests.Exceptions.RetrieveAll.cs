using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // Given
            User someUser = CreateRandomUser();
            User storageUser = someUser;
            User expectedUser = storageUser;

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
                this.userService.RetrieveAllUsersAsync();

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
    }
}
