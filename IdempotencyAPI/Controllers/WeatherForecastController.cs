using Microsoft.AspNetCore.Mvc;
using API.BLL.Service;
using API.Dal.Model;
using API.BLL.Model;
namespace IdempotencyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IPaymentService _paymentService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Route("PaymentService")]
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromHeader(Name ="Idempotency-key")] string idempotencyKey, 
            [FromBody] PaymentRequest request) {

            var response = await _paymentService.PaymentAPIService(idempotencyKey,request);
            if(response.statuscode == 409) {
                return StatusCode(StatusCodes.Status409Conflict,new { response.message});
            }

            return Ok(new {response.paymentReference,response.status});
        }
    }
}
