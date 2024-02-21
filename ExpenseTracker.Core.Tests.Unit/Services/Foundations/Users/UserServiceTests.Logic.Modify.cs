﻿using ExpenseTracker.Core.Models.Users;
using FluentAssertions;
using Force.DeepCloner;
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
        public async void ShouldModifyUserAsync()
        {
            // Given
            DateTimeOffset randomDate = 
                GetRandomDateTimeOffset();

            User randomUser = 
                CreateRandomUser(dates: randomDate);

            User inputUser = randomUser;
            User storageUser = inputUser;

            inputUser.UpdatedDate = randomDate.AddMinutes(1);
            User updatedUser = inputUser;
            User expectedUser = updatedUser.DeepClone();

            Guid inputUserId = inputUser.Id;

            //this.dateTimeBrokerMock.Setup(broker => 
            //    broker.GetCurrentDateTimeOffset())
            //        .Returns(randomDate);

            this.userManagerBrokerMock.Setup(broker => 
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(storageUser);

            this.userManagerBrokerMock.Setup(broker => 
                broker.UpdateUserAsync(inputUser))
                    .ReturnsAsync(storageUser);

            // When
            User actualUser = await this.userService.ModifyUserAsync(inputUser);

            // Then
            actualUser.Should().BeEquivalentTo(expectedUser);

            //this.dateTimeBrokerMock.Verify(broker => 
            //    broker.GetCurrentDateTimeOffset(), 
            //        Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(inputUserId), 
                    Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.UpdateUserAsync(inputUser), 
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
