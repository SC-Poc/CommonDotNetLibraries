using System.Threading.Tasks;

namespace Common
{
    public interface ISlackNotificationsSender
    {
        Task SendAsync(string type, string sender, string message);
    }
}
