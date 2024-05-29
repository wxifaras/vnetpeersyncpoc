using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Text.Json;

public class VnetPeerSync
{
    private readonly List<VNETPeeringSyncPoc.VnetSettings> _vnetSettings = new List<VNETPeeringSyncPoc.VnetSettings>();
    public readonly VNETPeerSyncPoc.AuthSettings _authSettings = new VNETPeerSyncPoc.AuthSettings();

    public VnetPeerSync(IConfiguration config)
    {
        config.GetSection("VnetSettings").Bind(_vnetSettings);
        config.GetSection("AuthSettings").Bind(_authSettings);
    }

    public async Task SyncPeering()
    {
        if (_vnetSettings.Any())
        {
            foreach (var setting in _vnetSettings)
            {
                try
                {
                    if (setting.IsValid())
                    {
                        // get your azure access token, for more details of how Azure SDK get your access token, please refer to https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication?tabs=command-line
                        var cred = new DefaultAzureCredential();

                        // authenticate your client
                        var client = new ArmClient(cred);

                        var virtualNetworkResourceId = VirtualNetworkResource.CreateResourceIdentifier(setting.SubscriptionOne, setting.ResourceGroupOne, setting.Vnet1);
                        var virtualNetwork = client.GetVirtualNetworkResource(virtualNetworkResourceId);

                        // get the collection of this VirtualNetworkPeeringResource
                        var collection = virtualNetwork.GetVirtualNetworkPeerings();

                        // invoke the operation
                        var data = new VirtualNetworkPeeringData()
                        {
                            AllowVirtualNetworkAccess = true,
                            AllowForwardedTraffic = true,
                            AllowGatewayTransit = false,
                            UseRemoteGateways = false,
                            RemoteVirtualNetworkId = new ResourceIdentifier($"/subscriptions/{setting.SubscriptionTwo}/resourceGroups/{setting.ResourceGroupTwo}/providers/Microsoft.Network/virtualNetworks/{setting.Vnet2}"),
                        };

                        Console.WriteLine($"Peer syncing: {collection.Id}");

                        var syncRemoteAddressSpace = SyncRemoteAddressSpace.True;
                        ArmOperation<VirtualNetworkPeeringResource> lro = await collection.CreateOrUpdateAsync(WaitUntil.Completed, setting.VnetPeerName, data, syncRemoteAddressSpace: syncRemoteAddressSpace);
                        VirtualNetworkPeeringResource result = lro.Value;

                        // the variable result is a resource, you could call other operations on this instance as well
                        // but just for demo, we get its data from this resource instance
                        VirtualNetworkPeeringData resourceData = result.Data;

                        string jsonString = JsonSerializer.Serialize(resourceData);

                        Console.WriteLine(jsonString);

                        Console.WriteLine($"Succeeded on id: {resourceData.Id}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid VnetSettings");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    private async Task<TokenCredential> GetCredential()
    {
        string authority = $"https://login.microsoftonline.com/{_authSettings.TenantId}";
        string clientId = _authSettings.ClientId;
        string clientSecret = _authSettings.ClientSecret;

        string[] scopes = new string[] { "https://management.azure.com/.default" };

        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri(authority))
            .Build();

        AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        string accessToken = result.AccessToken;
        var credential = new VNETPeerSyncPoc.SimpleTokenCredential(accessToken);
        return credential;
    }
}