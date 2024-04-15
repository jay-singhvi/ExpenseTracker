// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class NullUserException : Xeption
    {
        public NullUserException() : base(message: "User is Null.")
        { }

        public NullUserException(string message) : base(message: message)
        { }
    }
}