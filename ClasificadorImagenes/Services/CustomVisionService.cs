using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClasificadorImagenes.Services
{
    public class CustomVisionService
    {
        private readonly string _predictionKey;
        private readonly string _predictionEndpoint;
        private readonly string _predictionEndpointURL;

        public CustomVisionService(IConfiguration configuration)
        {
            _predictionKey = configuration["CustomVision:PredictionKey"];
            _predictionEndpoint = configuration["CustomVision:PredictionEndpoint"];
            _predictionEndpointURL = configuration["CustomVision:PredictionEndpoint2"];
        }

        public async Task<IList<PredictionModel>> ClassifyImageFromImageAsync(Stream imageStream)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", _predictionKey);

                using (var content = new StreamContent(imageStream))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = await client.PostAsync(_predictionEndpoint, content);

                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();

                    var jsonResponse = JObject.Parse(responseBody);
                    var predictions = jsonResponse["predictions"].ToObject<IList<PredictionModel>>();
                    Console.WriteLine(jsonResponse.ToString());
                    return predictions;
                }
            }
        }

        public async Task<IList<PredictionModel>> ClassifyImageFromUrlAsync(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", _predictionKey);

                var jsonContent = new StringContent(
                    $"{{ \"Url\": \"{imageUrl}\" }}",
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_predictionEndpointURL, jsonContent);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(responseBody);
                var predictions = jsonResponse["predictions"].ToObject<IList<PredictionModel>>();

                return predictions;
            }
        }
    }

    public class PredictionModel
    {
        public string TagName { get; set; }
        public double Probability { get; set; }
    }
}
