using Biss.MultiSinkLogger.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Biss.MultiSinkLogger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult GetWeather()
        {
            Logger.Info(LogMessages.WeatherForecastEndpointAccessed);
            return Ok("Previsão do tempo recuperada com sucesso.");
        }

        [HttpGet("error")]
        public IActionResult SimulateError()
        {
            throw new Exception("Exceção simulada");
        }

        [HttpGet("external")]
        public async Task<IActionResult> GetExternalData()
        {
            var client = _clientFactory.CreateClient("ExternalApi");
            var response = await client.GetAsync("https://api.github.com/");
            var content = await response.Content.ReadAsStringAsync();

            Logger.Info(LogMessages.ExternalApiDataRetrieved);
            return Ok(content);
        }
    }
}
