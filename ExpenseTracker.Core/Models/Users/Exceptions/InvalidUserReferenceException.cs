using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class InvalidUserReferenceException : Xeption
    {
        public InvalidUserReferenceException(Exception innerException) 
            : base(message:"Invalid user reference error occurred.",innerException)
        {}
    }
}
