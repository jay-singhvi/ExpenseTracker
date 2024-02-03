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
            ValidateUserFields(user);
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

        private void ValidateUserFields(User user)
        {
            if (IsInvalid(user.UserName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.UserName), 
                    parameterValue: user.UserName);
            }
        }

        private bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);
    }
}
