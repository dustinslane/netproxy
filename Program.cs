using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NetProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var configJson = System.IO.File.ReadAllText("config.json");

                var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ProxyConfig>>(configJson);

                List<Task> tasks =  new List<Task>();

                foreach ( var config in configs ) {
                    Console.WriteLine($"[Setup] Initialising Proxy {config.Key}");
                    tasks.Add(StartTCPClient(config.Value));
                    tasks.Add(StartUDPClient(config.Value));
                    Console.WriteLine("");
                }
                
                Task.WhenAll(tasks).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured : {ex}");
            }
        }
        private static Task StartTCPClient(ProxyConfig c) {
            return new TcpProxy().Start(c.forwardIp, c.forwardPort, c.localPort, c.localIp);
        }

        private static Task StartUDPClient(ProxyConfig c) {
            return new UdpProxy().Start(c.forwardIp, c.forwardPort, c.localPort, c.localIp);
        }
    }
    
    

    public class ProxyConfig
    {
        public ushort localPort { get; set; }
        public string localIp { get; set; }
        public string forwardIp { get; set; }
        public ushort forwardPort { get; set; }
    }
    interface IProxy
    {
        Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort, string localIp = null);
    }
}