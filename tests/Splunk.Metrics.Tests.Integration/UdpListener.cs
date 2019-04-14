using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Xunit.Abstractions;

namespace Splunk.Metrics.Tests.Integration
{   
    public class UdpListener : IDisposable 
    {
        private readonly ITestOutputHelper _testOutput;
        private int _expectedMessages;
        private readonly List<byte[]> _receivedBytes = new List<byte[]> ();
        private readonly UdpClient _udpClient;
        private readonly IPAddress _localIpAddress = IPAddress.Parse("127.0.0.1");
        private readonly ManualResetEventSlim _writtenEvent = new ManualResetEventSlim();
        private const string messageDelimiter = "&";
        
        public UdpListener(ITestOutputHelper testOutput, int expectedMessages = 1)
        {
            _testOutput = testOutput;
            _expectedMessages = expectedMessages;
            
            Port = Ports.GetFreePort();
            var uEndpoint = new IPEndPoint(_localIpAddress, Port);
            _udpClient = new UdpClient(new IPEndPoint(_localIpAddress, Port));
            _udpClient.BeginReceive (RxCallback, new UdpState(_udpClient, uEndpoint));
        }

        public int Port { get; }
        
        private void RxCallback(IAsyncResult result)
        {
            var udpClient = ((UdpState)result.AsyncState).Client;
            var ipEndpoint = ((UdpState)result.AsyncState).Endpoint;
            
            var receivedBytes = udpClient.EndReceive (result, ref ipEndpoint);
            if (_receivedBytes.Count == 0) _receivedBytes.Add(receivedBytes);
            else
            {
                _receivedBytes.Add(Encoding.UTF8.GetBytes(messageDelimiter));
                _receivedBytes.Add(receivedBytes);
            }
            
            _testOutput.WriteLine("Received Bytes ___________________________");
            _testOutput.WriteLine(receivedBytes.ToString ());

            if (--_expectedMessages == 0)
            {
                _writtenEvent.Set();
                return;
            }
            
            _udpClient.BeginReceive (RxCallback, new UdpState(_udpClient, ipEndpoint));
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

        public IEnumerable<string> GetWrittenBytesAsString()
        {
            _writtenEvent.Wait(2000);
            return Encoding.UTF8.GetString(_receivedBytes.SelectMany(bArray => bArray).ToArray()).Split(messageDelimiter);
        }
    }
}