using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.Metrics.Statsd.Channels
{
    internal sealed class UdpChannel : IChannel
    {
        private readonly UdpClient _udpClient;

        public UdpChannel(string hostOrIPAddress, int port)
        {
            _udpClient = new UdpClient();
            _udpClient.Connect(GetIpAddressFromHostname(hostOrIPAddress), port);
        }

        private static IPAddress GetIpAddressFromHostname(string hostOrIPAddress)
        {
            if (!IPAddress.TryParse(hostOrIPAddress, out var ipAddress))
            {
                ipAddress = Dns.GetHostAddresses(hostOrIPAddress).First(p => p.AddressFamily == AddressFamily.InterNetwork);
            }

            return ipAddress;
        }

        public void Send(string line)
        {
            var payload = Encoding.UTF8.GetBytes(line);
            _udpClient.Send(payload, payload.Length);
        }

        public async Task SendAsync(string line)
        {
            var payload = Encoding.UTF8.GetBytes(line);
            await _udpClient.SendAsync(payload, payload.Length);
        }
    }
}