using System.Threading.Tasks;

namespace Common
{
    public interface IServiceHealthMonitoring
    {
        Task HealthPingAsync();
    }
}
