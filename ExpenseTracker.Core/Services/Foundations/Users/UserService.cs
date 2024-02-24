using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.UserManagers;
using ExpenseTracker.Core.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IUserManagerBroker userManagerBroker;

        public UserService(IUserManagerBroker userManagerBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.userManagerBroker = userManagerBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;            
        }

        public ValueTask<User> RegisterUserAsync(User user, string password) =>
            TryCatch(async () => {
                ValidateUserOnAdd(user);
                return await this.userManagerBroker.InsertUserAsync(user, password);
            });

        public IQueryable<User> RetrieveAllUsers() =>
            TryCatch(() =>
            {
                return this.userManagerBroker.SelectAllUsers();
            });

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            TryCatch(async () => {
                ValidateUserIdIsNull(userId);
                User storageUser = await this.userManagerBroker.SelectUserByIdAsync(userId);
                ValidateStorageUser(storageUser, userId);

                return storageUser;
            });

        public ValueTask<User> ModifyUserAsync(User user) =>
            TryCatch(async () => {
                ValidateUserOnModify(user);

                User maybeUser =
                    await this.userManagerBroker.SelectUserByIdAsync(user.Id);

                return await this.userManagerBroker.UpdateUserAsync(user);
            });

        public ValueTask<User> RemoveUserByIdAsync(Guid userId) =>
            TryCatch(async () => { 

                ValidateUserIdIsNull(userId);

                User mayBeUser =
                    await this.userManagerBroker.SelectUserByIdAsync(userId);

                return await this.userManagerBroker.DeleteUserAsync(mayBeUser);
            });
    }
}
