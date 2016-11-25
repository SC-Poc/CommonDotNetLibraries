using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.IocContainer
{
    public interface IDependencyBinder
    {
	    ContainerBuilder Bind(IConfigurationRoot configuration);	    
    }
}
