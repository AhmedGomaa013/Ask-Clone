using Ask_Clone.Jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Threading.Tasks;

namespace Ask_Clone.Installers
{
    public class JwtAuthenticationInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            //JWT Installer
            var jwtSettings = new JWTSettings();
            configuration.Bind("jwtSettings", jwtSettings);

            services.AddSingleton(jwtSettings);
            services.AddTransient<JWTCreator>();

            //Authentication
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Token").ToString());

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };
                opt.SaveToken = true;
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("Token"))
                        {
                            context.Token = context.Request.Cookies["Token"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
