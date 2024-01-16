using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class UserValidationException : Xeption
    {
        public UserValidationException(Xeption innerException) :
            base(message: "User Validation error occurred, please try again.", innerException)
        { }
    }
}
