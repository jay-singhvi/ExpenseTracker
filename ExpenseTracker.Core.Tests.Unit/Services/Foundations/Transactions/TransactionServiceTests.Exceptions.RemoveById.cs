﻿using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // Given
            Guid randomTransactionId = Guid.NewGuid();
            var sqlException = GetSqlException();

            var failTransactionStorageException =
                new FailedTransactionStorageException(sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(failTransactionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(randomTransactionId))
                    .ThrowsAsync(sqlException);

            // When
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(randomTransactionId);

            var actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    removeTransactionByIdTask.AsTask);

            // Then
            actualTransactionDependencyException.Should()
                .BeEquivalentTo(expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDBUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // Given
            Guid someTransactionID = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedTransactionException = 
                new LockedTransactionException(dbUpdateConcurrencyException);

            var expectedTransactionDependencyValidationException = 
                new TransactionDependencyValidationException(lockedTransactionException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectTransactionByIdAsync(someTransactionID))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // When
            ValueTask<Transaction> removeByIdTask = 
                this.transactionService.RemoveTransactionByIdAsync(someTransactionID);

            var actualTransactionDependencyValidationException = 
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(removeByIdTask.AsTask);

            // Then
            actualTransactionDependencyValidationException.Should()
                .BeEquivalentTo(expectedTransactionDependencyValidationException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()), 
                        Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTransactionDependencyValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            
        }
    }
}