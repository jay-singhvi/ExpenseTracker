// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class InvalidUserException : Xeption
    {
        public InvalidUserException(string parameterName, object parameterValue) :
            base(message: $"Invalid user,"
            + $"parameter name: {parameterName}"
            + $"parameter value: {parameterValue}")
        { }
    }
}
