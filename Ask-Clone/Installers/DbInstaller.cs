using Ask_Clone.Models;
using Ask_Clone.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ask_Clone.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            //Install Datebase
            services.AddDbContext<AuthenticationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            //Install Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationContext>()
                .AddDefaultTokenProviders();

            //Install Repositories
            services.AddScoped<IQuestionsRepository, QuestionsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
