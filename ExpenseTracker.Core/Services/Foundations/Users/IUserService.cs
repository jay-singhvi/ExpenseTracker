using ExpenseTracker.Core.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public interface IUserService
    {
        ValueTask<User> RegisterUserAsync(User user, string password);
        IQueryable<User> RetrieveAllUsers();
        ValueTask<User> RetrieveUserById(Guid userId);
    }
}
