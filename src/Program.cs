using Microsoft.Extensions.Configuration;

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
                var sync = new VnetPeerSync(configuration);
                await sync.SyncPeering();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
