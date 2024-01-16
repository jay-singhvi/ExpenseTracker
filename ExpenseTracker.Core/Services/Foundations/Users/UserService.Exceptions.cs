using ExpenseTracker.Core.Models.Transactions.Exceptions;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using System.Threading.Tasks;
using Xeptions;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningPostFunction();

        private async ValueTask<User> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw CreateAndLogValidationException(nullUserException);
            }
        }

        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userValidationException = 
                new UserValidationException(exception);

            this.loggingBroker.LogError(userValidationException);

            return userValidationException;
        }
    }
}
