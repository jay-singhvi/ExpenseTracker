using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class AlreadyExistsUserException : Xeption
    {
        public AlreadyExistsUserException(Exception innerException)
            : base(message: "User with same Id already exists.", innerException)
        { }
    }
}
