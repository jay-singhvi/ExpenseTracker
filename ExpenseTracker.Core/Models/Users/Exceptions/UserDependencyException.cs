// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(Xeption innerException)
            : base(message: "User dependency error occurred, contact support.", innerException)
        { }
    }
}
