using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace AspNetCoreAppMetrics.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public WeatherForecast Get()
        {
            var rng = new Random();
            var forecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = rng.Next(-20, 30),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

            var histogram = Metrics.CreateHistogram(
                "temperature_with_summary",
                "Temperature buckets divided by summary",
                new HistogramConfiguration
                {
                    LabelNames = new []{"Summary"},
                    Buckets = Histogram.LinearBuckets(-20, 10, 6)
                }
            );
            histogram
                .WithLabels(forecast.Summary)
                .Observe(forecast.TemperatureC);
            
            return forecast;
        }
    }
}
