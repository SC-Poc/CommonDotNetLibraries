using Autofac;
using Microsoft.Extensions.Configuration;

namespace Common.IocContainer
{
    public interface IDependencyBinder
    {
	    ContainerBuilder Bind(IConfigurationRoot configuration, ContainerBuilder containerBuilder = null);	    
    }
}
