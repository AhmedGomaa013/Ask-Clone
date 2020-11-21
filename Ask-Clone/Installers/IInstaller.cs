using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ask_Clone.Installers
{
    public interface IInstaller
    {
        void InstallService(IServiceCollection services, IConfiguration configuration);
    }
}
