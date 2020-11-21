using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Ask_Clone.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(opt =>
            opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize);
        }
    }
}
