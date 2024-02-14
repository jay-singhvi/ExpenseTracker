using ExpenseTracker.Core.Models.Users;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Brokers.UserManagers
{
    public interface IUserManagerBroker
    {
        ValueTask<User> InsertUserAsync(User user, string password);
        IQueryable<User> SelectAllUsers();
    }
}
