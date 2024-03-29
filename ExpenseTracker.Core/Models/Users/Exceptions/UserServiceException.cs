// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class UserServiceException : Xeption
    {
        public UserServiceException(Xeption innerException)
            : base(message: "Profile service error occurred, contact support.", innerException)
        { }

        public UserServiceException(string message, Xeption innerException)
            : base(message: message, innerException)
        { }
    }
}
