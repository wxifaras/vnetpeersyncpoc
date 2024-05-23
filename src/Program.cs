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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
                var vnetSettings = new List<VnetSettings>();
                
                configuration.GetSection("VnetSettings").Bind(vnetSettings);

                if (vnetSettings.Any())
                {
                    var sync = new VnetPeerSync();
                    await sync.SyncPeering(vnetSettings);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
