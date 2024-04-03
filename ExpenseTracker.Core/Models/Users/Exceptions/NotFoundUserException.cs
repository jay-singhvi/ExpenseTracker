// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(Guid userId) 
            : base(message: $"Coundn't find user with id: {userId}")
        { }

        public NotFoundUserException(string message, Guid userId)
            : base(message: message)
        { }
    }
}
