namespace DemoOpenTelemetry
{
    using System.Diagnostics;
    using Azure.Monitor.OpenTelemetry.Exporter;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTelemetry.Logs;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;

    /// <summary>
    /// Startup class contains method for App Startiup
    /// </summary>
    public static class Startup
    {
        public static void SetCustomerInstanceConfigurations(WebApplicationBuilder builder)
        {
            builder.Configuration.AddEnvironmentVariables(prefix: "CustomerInstanceConfiguration_");
            builder.Services.Configure<CustomerInstanceConfigurationOptions>(builder.Configuration.GetSection("CustomerInstanceConfigurationOptions"));
        }

        /// <summary>
        /// Method to inject services in to the DI Container
        /// </summary>
        /// <param name="services">Service Collection</param>
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            //Configure Options
            services.ConfigureOptions(configuration);
        }

        public static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
        {
            var appInsightsConnection = builder.Configuration.GetSection("ApplicationInsights")["ConnectionString"];
            var customerInstanceConfig = builder.Configuration.GetSection("CustomerInstanceConfiguration").Get<CustomerInstanceConfigurationOptions>();

            builder.Services.AddOpenTelemetry()
                .WithTracing(builder => builder
                            .AddAspNetCoreInstrumentation(o =>
                            {
                                o.EnrichWithHttpRequest = (activity, httpRequest) =>
                                {
                                    activity.SetTag("requestProtocol", httpRequest.Protocol);
                                };
                                o.EnrichWithHttpResponse = (activity, httpResponse) =>
                                {
                                    activity.SetTag("responseLength", httpResponse.ContentLength);
                                };
                                o.EnrichWithException = (activity, exception) =>
                                {
                                    activity.SetTag("exceptionType", exception.GetType().ToString());
                                };
                            })
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddAzureMonitorTraceExporter(o =>
                    {
                        o.ConnectionString = appInsightsConnection;
                        o.Diagnostics.IsDistributedTracingEnabled = true;
                    }))
                .WithMetrics(builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddAzureMonitorMetricExporter(o =>
                    {
                        o.ConnectionString = appInsightsConnection;
                    })
                    .ConfigureResource(resource =>
                    {
                        resource.AddService(DiagnosticsConfig.ServiceName);
                        resource.AddAttributes(customerInstanceConfig.Values);
                    }))
                .ConfigureResource(resource =>
                {
                    resource.AddService(DiagnosticsConfig.ServiceName);
                    resource.AddAttributes(customerInstanceConfig.Values);
                }
                );
        }
    }

    public static class DiagnosticsConfig
    {
        public const string ServiceName = "DemoOpenTelemetry";
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
