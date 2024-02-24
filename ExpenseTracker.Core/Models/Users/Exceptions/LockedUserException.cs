using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class LockedUserException : Xeption
    {
        public LockedUserException(Exception innerException)
            : base(message: "Locked user record exception, please try again.", innerException)
        { }
    }
}
