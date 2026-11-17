using System;
using System.Threading.Tasks;

namespace LearnBackendSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Starting Backend Server ===");
            var server = new NetworkServer();

            // Chạy server loop ở background task
            Task.Run(() => server.Start(9050));

            // Bắt sự kiện khi nhấn Ctrl + C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\n[App] Shutting down server...");
                server.Stop();
            };

            Console.WriteLine("[App] Server is running. Press 'q' or 'exit' and Enter to quit.\n");
            while (true)
            {
                var input = Console.ReadLine();
                if (input?.Trim().ToLower() is "exit" or "q")
                {
                    server.Stop();
                    break;
                }
            }
        }
    }
}
