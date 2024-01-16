using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.UserManagers;
using ExpenseTracker.Core.Models.Users;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public class UserService : IUserService
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

        public ValueTask<User> RegisterUserAsync(User user, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}
