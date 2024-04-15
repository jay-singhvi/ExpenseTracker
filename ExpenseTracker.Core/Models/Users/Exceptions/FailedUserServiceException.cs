// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class FailedUserServiceException : Xeption
    {
        public FailedUserServiceException(Exception innerException)
            : base(message: "Failed user service error occurred, please contact support.", innerException)
        { }

        public FailedUserServiceException(string message, Exception innerException)
            : base(message: message, innerException)
        { }
    }
}
