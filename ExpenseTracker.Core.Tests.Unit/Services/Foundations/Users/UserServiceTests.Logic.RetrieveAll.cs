// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Moq;
using System.Linq;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUsersAsync()
        {
            // Given
            IQueryable<User> randomUsers = CreateRandomUsers();
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
