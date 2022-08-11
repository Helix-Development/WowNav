using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace WowNavApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string listeningHostAndPort;
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: WowNavApi.exe <listeningHostAndPort> (ie 'WowNavApi.exe http://localhost:5000')");
                Console.WriteLine("Defaulting to http://localhost:5000");
                listeningHostAndPort = "http://localhost:5000";
            }
            else
            {
                listeningHostAndPort = args[0];
            }

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(listeningHostAndPort);
                });
        }
    }
}
