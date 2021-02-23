using System;
using System.Threading.Tasks;
using MyServiceBus.TcpClient;
using Newtonsoft.Json;
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

            serviceBusClient.Start();

            var subs = new MeEventServiceBusSubscriber(serviceBusClient, "Test-App-1", true);

            subs.Subscribe(meEventList =>
            {
                foreach (var meEvent in meEventList)
                {
                    Console.WriteLine($"{meEvent.Header.EventType}: {meEvent.Header.SequenceNumber}");
                    Console.WriteLine(JsonConvert.SerializeObject(meEvent, Formatting.Indented));

                    //Console.ReadLine();
                }
                return ValueTask.CompletedTask;
            });

            




            Console.WriteLine("End");
            Console.ReadLine();

            serviceBusClient.Stop();
        }
    }
}
