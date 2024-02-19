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
        public async void ShouldRetrieveUserById()
        {
            // Given
            User someUser = CreateRandomUser();
            User storageUser = someUser;
            User expectedUser = storageUser;
            Guid userId = someUser.Id;

            this.userManagerBrokerMock.Setup(broker => 
                broker.SelectUserById(userId))
                    .ReturnsAsync(storageUser);

            // When
            User actualUser = 
                await this.userService.RetrieveUserByIdAsync(userId);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserById(It.IsAny<Guid>()), 
                    Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
