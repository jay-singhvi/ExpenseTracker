// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class AlreadyExistsUserException : Xeption
    {
        public AlreadyExistsUserException(Exception innerException)
            : base(message: "User with same Id already exists.", innerException)
        { }

        public AlreadyExistsUserException(string message, Exception innerException)
            : base(message: message, innerException)
        { }
    }
}
