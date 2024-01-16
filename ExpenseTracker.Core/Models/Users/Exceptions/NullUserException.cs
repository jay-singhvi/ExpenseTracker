using Xeptions;

namespace ExpenseTracker.Core.Models.Users.Exceptions
{
    public class NullUserException : Xeption
    {
        public NullUserException() : base(message: "User is Null.")
        { }
    }
}