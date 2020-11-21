using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Ask_Clone.Installers
{
    public static class InstallServices
    {
        public static void InstallAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            
            installers.ForEach(installer => installer.InstallService(services, configuration));
        }
    }
}
