using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Splunk.Metrics.Tests.Integration
{   
    public class UdpListener : IDisposable 
    {
        private readonly List<byte[]> _receivedBytes = new List<byte[]> ();
        private readonly UdpClient _udpClient;
        private readonly IPAddress localIpAddress = IPAddress.Parse("127.0.0.1");
        
        public UdpListener()
        {
            Port = Ports.GetFreePort();
            var uEndpoint = new IPEndPoint(localIpAddress, Port);
            _udpClient = new UdpClient(new IPEndPoint(localIpAddress, Port));
            _udpClient.BeginReceive (RxCallback, new UdpState(_udpClient, uEndpoint));
        }

        public int Port { get; }
        
        private void RxCallback(IAsyncResult result)
        {
            var udpClient = ((UdpState)result.AsyncState).Client;
            var ipEndpoint = ((UdpState)result.AsyncState).Endpoint;
            
            var receivedBytes = udpClient.EndReceive (result, ref ipEndpoint);
            _receivedBytes.Add (receivedBytes);
            
            Console.WriteLine ("Received Bytes ___________________________");
            Console.WriteLine (receivedBytes.ToString ());
        }

        private class UdpState
        {
            public UdpClient Client { get; }
            public IPEndPoint Endpoint { get; }

            public UdpState(UdpClient udpClient, IPEndPoint ipEndpoint)
            {
                Client = udpClient;
                Endpoint = ipEndpoint;
            }
        }
        
        public void Dispose() => _udpClient?.Dispose();

        public string GetWrittenBytesAsString() => Encoding.UTF8.GetString(_receivedBytes.SelectMany(bArray => bArray).ToArray());
    }
}