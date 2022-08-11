using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WowNavBase;

namespace WowNavClient
{
    public class NavClient
    {
        private readonly HttpClient httpClient = new HttpClient();

        public NavClient(Uri wowNavApiUri)
        {
            httpClient.BaseAddress = wowNavApiUri;
        }

        public async Task<Position[]> CalculatePath(uint mapId, Position start, Position end, bool straightPath)
        {
            var parameters = new
            {
                mapId = mapId,
                start = new
                {
                    X = start.X,
                    Y = start.Y,
                    Z = start.Z
                },
                end = new
                {
                    X = end.X,
                    Y = end.Y,
                    Z = end.Z
                },
                straightPath = straightPath
            };

            var response = await httpClient.PostAsync("/Navigation/CalculatePath", new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"An error occurred while calling the WowNavApi. ResponseCode={response.StatusCode} ReasonPhrase={response.ReasonPhrase}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var path = JsonConvert.DeserializeObject<Position[]>(content);
            return path;
        }
    }
}
