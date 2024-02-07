using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class FailedUserStorageException : Xeption
    {
        public FailedUserStorageException(Exception innerException)
            : base(message: "Failed user storage error occurred, contact support.", innerException)
        { }
    }
}
