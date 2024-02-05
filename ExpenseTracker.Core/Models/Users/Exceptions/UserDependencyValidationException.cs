using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class UserDependencyValidationException : Xeption
    {
        public UserDependencyValidationException(Xeption innerException) 
            : base(message: "User dependency validation occurred, please try again.", innerException)
        {}
    }
}
