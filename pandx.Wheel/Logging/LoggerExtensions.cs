using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Settings.Configuration;

namespace pandx.Wheel.Logging;

public static class LoggerExtensions
{
    public static WebApplicationBuilder ConfigureLogger(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration,
                new ConfigurationReaderOptions { SectionName = "LoggerSettings:Serilog" })
            .ReadFrom.Services(services));
        return builder;
    }
}