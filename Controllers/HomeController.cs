using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReportML.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace ReportML.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(ModelParams model)
        {
            var prediction = await GetPredictionFromAzureML(model);
            ViewBag.Prediction = prediction;

            // Parse the "Scored Labels" from the prediction response and convert to int
            var predictionJson = JObject.Parse(prediction);
            var scoredLabels = (int)predictionJson["Results"]["WebServiceOutput0"][0]["Scored Labels"].Value<double>();
            ViewBag.ScoredLabels = scoredLabels;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetPredictionFromAzureML(ModelParams model)
        {
            using (var client = new HttpClient())
            {
                var requestUri = "http://4dce101b-f502-48fa-99f3-31c14f4f5c39.israelcentral.azurecontainer.io/score";
                var apiKey = "tSDZT5IcNSLkrjsagjR9E8pAVcX00L5P";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    Inputs = new
                    {
                        input1 = new[]
                        {
                            new
                            {
                                context_message_count = model.ContextMessageCount,
                                battery_voltage = model.BatteryVoltage,
                                battery_voltageLoaded = model.BatteryVoltageLoaded,
                                context_message_count1 = model.ContextMessageCount1,
                                softwareVersion = model.SoftwareVersion,
                                firmwareVersion = model.FirmwareVersion,
                                flightMode = model.FlightMode,
                                GpsMode = model.GpsMode,
                                movementSens = model.MovementSens,
                                movementInterval = model.MovementInterval,
                                staticInterval = model.StaticInterval,
                                sealDetectionEnabled = model.SealDetectionEnabled,
                                durationThreshold = model.DurationThreshold,
                                magnitudeThreshold = model.MagnitudeThreshold,
                                vdiff = model.Vdiff,
                                temperatureLoggingInterval = model.TemperatureLoggingInterval
                            }
                        }
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(payload);
                _logger.LogInformation("Request Payload: {0}", jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error Response: {0}", responseString);
                    response.EnsureSuccessStatusCode();
                }

                return responseString;
            }
        }
    }
}
