using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;

namespace DemoOpenTelemetry.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private static readonly Meter meter = new("DemoOpenTelemetry.Privacy");

        public PrivacyModel(ILogger<PrivacyModel> logger, IOptions<CustomerInstanceConfigurationOptions> options)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("Privacy page visited");

            Counter<long> myPrivacyCounter = meter.CreateCounter<long>("MyPrivacyCounter");

            myPrivacyCounter.Add(1, new("name", "apple"), new("color", "red"));
            myPrivacyCounter.Add(2, new("name", "lemon"), new("color", "yellow"));
            myPrivacyCounter.Add(1, new("name", "lemon"), new("color", "yellow"));
            myPrivacyCounter.Add(2, new("name", "apple"), new("color", "green"));
            myPrivacyCounter.Add(5, new("name", "apple"), new("color", "red"));
            myPrivacyCounter.Add(4, new("name", "lemon"), new("color", "yellow"));
        }
    }
}