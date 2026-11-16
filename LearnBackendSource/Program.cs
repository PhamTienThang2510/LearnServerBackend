using System;
using System.Threading.Tasks;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using NetworkShare;
using LearnBackendSource.Packets;
using Infrastructure;
namespace LearnBackendSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Starting Backend Server ===");
            var container = Container.Configure();
            var server = container.GetService<NetworkServer>();


            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\n[App] Shutting down server...");
                server.Stop();
            };

            server.Start(9050);
        }
    }
}
