using System.Threading.Tasks;

namespace Splunk.Metrics.Statsd.Channels
{
    public interface IChannel
    {
        void Send(string message);
        Task SendAsync(string message);
    }
}