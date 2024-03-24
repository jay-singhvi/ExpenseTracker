// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Brokers.UserManagers
{
    public interface IUserManagerBroker
    {
        ValueTask<User> InsertUserAsync(User user, string password);
        IQueryable<User> SelectAllUsers();
        ValueTask<User> SelectUserByIdAsync(Guid userId);
        ValueTask<User> UpdateUserAsync(User user);
        ValueTask<User> DeleteUserAsync(User user);

    }
}
