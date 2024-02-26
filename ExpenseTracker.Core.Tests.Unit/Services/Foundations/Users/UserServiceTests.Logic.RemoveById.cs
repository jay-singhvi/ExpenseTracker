using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async void ShouldRemoveUserByIdAsync()
        {
            // Given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User storageUser = inputUser;
            User deletedUser = storageUser;
            User expecteddeletedUser = deletedUser.DeepClone();

            this.userManagerBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.userManagerBrokerMock.Setup(broker =>
                broker.DeleteUserAsync(storageUser))
                    .ReturnsAsync(deletedUser);

            // When
            var actualdeletedUser =
                await this.userService.RemoveUserByIdAsync(inputUser.Id);

            // Then
            actualdeletedUser.Should().BeEquivalentTo(expecteddeletedUser);

            this.userManagerBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUser.Id),
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker =>
                broker.DeleteUserAsync(inputUser),
                    Times.Once);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
