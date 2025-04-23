using System.Diagnostics;
using System.Net.Mime;
using MpesaIntegration.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MpesaIntegration.Models;
using Newtonsoft.Json;

namespace MpesaIntegration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> GetToken()
        {
            var client = _clientFactory.CreateClient("mpesa");

            var authString = "rVJdRBdIoL2ekxlJEneg4GesCR6glsqwrK4zZHM0WzKAEopK:TxKGrH7UtqhnOZxVwfYi75iUSL73D2ipK6bSZWQSaGdDRHwS1srX62ldq7GNVEM0";

            var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));

            var _url = "/oauth/v1/generate?grant_type=client_credentials";

            var request = new HttpRequestMessage(HttpMethod.Get, _url);

            request.Headers.Add("Authorization", $"Basic {encodedString}");

            var response = await client.SendAsync(request);

            var mpesaResponse = await response.Content.ReadAsStringAsync();

            Token tokenObject = JsonConvert.DeserializeObject<Token>(mpesaResponse);

            return tokenObject.access_token;
        }

        class Token
        {
            public string access_token {  get; set; }
            public string expires_in {  get; set; }
        }

        //register url
        public IActionResult RegisterURLs()
        {
            return View();
        }

        [HttpGet]
        [Route("register-urls")]
        public async Task<string> RegisterMpesaURLs()
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                validationURL = "https://e0a7-41-209-57-168.ngrok-free.app/validation",
                ConfirmationURL = "https://e0a7-41-209-57-168.ngrok-free.app/confirmation",
                ResponseType = "Completed",
                Shortcode = 600983
            });

            var jsonReadyBody = new StringContent(
                jsonBody.ToString(),
                Encoding.UTF8,
                "application/json"
                );
            var token = await GetToken();

            var client = _clientFactory.CreateClient("mpesa");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var url = "/mpesa/c2b/v1/registerurl";

            var response = await client.PostAsync(url, jsonReadyBody);

            return await response.Content.ReadAsStringAsync();
        }

        // Confirmation Endpoint
        [HttpPost]
        [Route("payments/confirmation")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<string> PaymentConfirmation([FromBody] MpesaC2B c2bPayments)
        {

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
    }
}
