using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using System;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserIsNotNull(user);
            ValidateUserIdIsNull(user.Id);
        }

        private void ValidateUserIsNotNull(User user)
        {
            if (user == null)
            {
                throw new NullUserException();
            }
        }

        private static void ValidateUserIdIsNull(Guid userId)
        {
            if (userId == default)
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.Id),
                    parameterValue: userId);
            }
        }
    }
}
