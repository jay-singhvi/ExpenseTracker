
using ExpenseTracker.Core.Brokers.DateTimes;
using ExpenseTracker.Core.Brokers.Loggings;
using ExpenseTracker.Core.Brokers.Storages;
using ExpenseTracker.Core.Brokers.UserManagers;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Services.Foundations.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpenseTracker.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
            builder.Services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            builder.Services.AddTransient<IStorageBroker, StorageBroker>();
            builder.Services.AddTransient<IUserManagerBroker, UserManagerBroker>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();

            builder.Services.AddIdentityApiEndpoints<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<StorageBroker>();

            builder.Services.AddDbContext<StorageBroker>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapIdentityApi<User>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
