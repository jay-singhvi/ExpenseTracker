using ExpenseTracker.Core.Models.Users;
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
        public async void ShouldRetrieveAllUsersAsync()
        {
            // Given
            IQueryable<User>  randomUsers = CreateRandomUsers();
            IQueryable<User> storageUsers = randomUsers;
            IQueryable<User> expectedUsers = storageUsers;

            this.userManagerBrokerMock.Setup(broker => 
                broker.SelectAllUsers())
                    .Returns(storageUsers);

            // When
            IQueryable<User> actualUsers = 
                this.userService.RetrieveAllUsers();

            // Then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectAllUsers(),
                    Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
