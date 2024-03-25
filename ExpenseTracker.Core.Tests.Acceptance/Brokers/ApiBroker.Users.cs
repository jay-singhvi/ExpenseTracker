// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string UsersRelativeUrl = "api/users";

        public async ValueTask<List<User>> GetAllUsersAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<User>>($"{UsersRelativeUrl}/");

        //public async ValueTask<User> PostUserAsync(User user)
        //{
        //    var registerRequest = new UserRequest
        //    {
        //        Email = user.Email,
        //        Password = user.PasswordHash
        //    };

        //    await this.apiFactoryClient.PostContentWithNoResponseAsync("/register", registerRequest, "application/json");

        //    return user;
        //}

        //public async ValueTask<User> LoginUserAsync(User user)
        //{
        //    var loginRequest =
        //        new UserRequest { Email = user.Email, Password = "*1Mar1988#"/*user.PasswordHash*/ };

        //    UserResponse response =
        //        await this.apiFactoryClient.PostContentAsync<UserRequest, UserResponse>("/login", loginRequest, "application/json");

        //    ConfigureHttpClient(response.AccessToken);

        //    return user;
        //}

        //private void ConfigureHttpClient(string accessToken)
        //{
        //    this.httpClient.DefaultRequestHeaders.Authorization =
        //        new AuthenticationHeaderValue("Bearer", accessToken);
        //}

    }
}
