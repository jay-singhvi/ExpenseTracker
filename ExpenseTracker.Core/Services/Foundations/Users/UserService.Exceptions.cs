using EFxceptions.Models.Exceptions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException = 
                    new FailedUserStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserStorageException);
            }

            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new AlreadyExistsUserException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsUserException);
            }
            //catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            //{
            //    var invalidUserReferenceException = 
            //        new InvalidUserReferenceException(foreignKeyConstraintConflictException);

            //    throw CreateAndLogDependencyValidationException(invalidUserReferenceException);
            //}
        }

        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userValidationException = 
                new UserValidationException(exception);

            this.loggingBroker.LogError(userValidationException);

            return userValidationException;
        }

        private UserDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var userDependencyValidationException = 
                new UserDependencyValidationException(exception);

            this.loggingBroker.LogError(userDependencyValidationException);

            return userDependencyValidationException;
        }

        private UserDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var userDependencyException = 
                new UserDependencyException(exception);

            this.loggingBroker.LogError(userDependencyException);

            return userDependencyException;
        }
    }
}
