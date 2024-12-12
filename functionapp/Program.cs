using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

var builder = FunctionsApplication.CreateBuilder(args);

ConfigureBuilder(builder);

await builder.Build().RunAsync();

static void ConfigureBuilder(FunctionsApplicationBuilder builder)
{
    ConfigureTelemetry(builder);
    builder.ConfigureFunctionsWebApplication();
}

static void ConfigureTelemetry(FunctionsApplicationBuilder builder)
{
    var telemetryBuilder = builder.Services.AddOpenTelemetry();

    if (builder.Configuration.TryGetKeyValue("APPLICATIONINSIGHTS_CONNECTION_STRING", out var _))
    {
        telemetryBuilder.UseAzureMonitor();
    }

    telemetryBuilder.UseFunctionsWorkerDefaults();
}

file static class ConfigurationModule
{
    public static bool TryGetKeyValue(this IConfiguration configuration, string key, [NotNullWhen(true)] out string? value)
    {
        var section = configuration.GetSection(key);

        if (section.Exists() && section.Value is not null)
        {
            value = section.Value;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }
}