using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyServiceBus.TcpClient;
using Newtonsoft.Json;
using ProtoBuf.Grpc.Client;
using Service.MatchingEngine.EventBridge.ServiceBus;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Press enter to start");
            Console.ReadLine();

            var serviceBusClient = new MyServiceBusTcpClient(() => "192.168.10.80:6421", "MyTestApp");


            var subs = new MeEventServiceBusSubscriber(serviceBusClient, "Test-App", true);

            subs.Subscribe(meEvent =>
            {
                Console.WriteLine($"{meEvent.Header.EventType}:");
                Console.WriteLine(JsonConvert.SerializeObject(meEvent, Formatting.Indented));
                return ValueTask.CompletedTask;
            });

            serviceBusClient.Start();




            Console.WriteLine("End");
            Console.ReadLine();

            serviceBusClient.Stop();
        }
    }
}
