using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VNETPeeringSyncPoc
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("Program", LogLevel.Debug)
                        .AddConsole();
                });

                var logger = loggerFactory.CreateLogger<VNetPeer>();
                var sync = new VNetPeer(logger);
                var vnetSettings = new List<VNetSettings>();
                configuration.GetSection("VnetSettings").Bind(vnetSettings);
                await sync.SyncVnetPeers(vnetSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}