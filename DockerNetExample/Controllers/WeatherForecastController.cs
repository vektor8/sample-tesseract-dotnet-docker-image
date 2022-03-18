using Microsoft.AspNetCore.Mvc;
using Tesseract;
namespace DockerNetExample.Controllers;

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
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    
    [HttpPost("upload")]
    public string UploadImage(IFormFile file)
    {
        var ms = new MemoryStream();
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        using (var engine = new TesseractEngine(@"/usr/share/tesseract-ocr/4.00/tessdata", "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromMemory(fileBytes))
            {
                using (var page = engine.Process(img))
                {
                    var text = page.GetText();
                    return text;
                }
            }
        }
    }
}
