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
            ValidateInvalidAuditFields(user);
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

            if (IsInvalid(user.FirstName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.FirstName),
                    parameterValue: user.FirstName);
            }

            if (IsInvalid(user.LastName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.LastName),
                    parameterValue: user.LastName);
            }

            if (IsInvalid(user.CreatedDate))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: user.CreatedDate);
            }

        }

        private static void ValidateInvalidAuditFields(User user)
        {
            switch (user)
            {
                case { } when IsInvalid(user.CreatedDate):
                    throw new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: user.CreatedDate);
                //case { } when IsInvalid(user.UpdatedDate):
                //    throw new InvalidUserException(
                //    parameterName: nameof(User.UpdatedDate),
                //    parameterValue: user.UpdatedDate);
            }
        }

        private static bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);

        private static bool IsInvalid(DateTimeOffset input) => input == default;
    }
}
