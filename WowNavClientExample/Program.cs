using System;
using System.Threading.Tasks;
using WowNavBase;
using WowNavClient;

namespace WowNavClientExample
{
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Calculating path...");

            // this should be whatever hostname/port you specify while starting the WowNavApi
            var navClient = new NavClient(new Uri("http://localhost:5000"));

            var start = new Position(-614.7f, -4335.4f, 40.4f);
            var end = new Position(-590.2f, -4206.1f, 38.7f);
            var path = await navClient.CalculatePath(1, start, end, false);

            if (path == null)
            {
                Console.WriteLine("Calculating path failed. See HttpClient logs.");
            }
            else
            {
                Console.WriteLine($"Path calculated successfully. Length={path.Length}");
            }
        }
    }
}
