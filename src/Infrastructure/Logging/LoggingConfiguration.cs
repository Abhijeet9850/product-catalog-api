using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Logging
{
    public static class LoggingConfiguration
    {
        public static IHostBuilder UseAppSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            return hostBuilder.UseSerilog();
        }
    }
}