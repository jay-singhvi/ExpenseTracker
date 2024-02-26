using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class UserServiceException : Xeption
    {
        public UserServiceException(Xeption innerException)
            : base(message: "Profile service error occurred, contact support.", innerException)
        { }
    }
}
