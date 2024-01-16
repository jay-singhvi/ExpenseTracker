using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserIsNotNull(user);
        }

        private void ValidateUserIsNotNull(User user)
        {
            if (user == null)
            {
                throw new NullUserException();
            }
        }
    }
}
