using ExpenseTracker.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Brokers.UserManagers
{
    public class UserManagerBroker : IUserManagerBroker
    {
        private readonly UserManager<User> userManager;

        public UserManagerBroker(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async ValueTask<User> InsertUserAsync(User user, string password)
        {
            var broker = new UserManagerBroker(this.userManager);
            await broker.userManager.CreateAsync(user, password);

            return user;
        }

        public IQueryable<User> SelectAllUsers() =>
            this.userManager.Users;

    }
}
