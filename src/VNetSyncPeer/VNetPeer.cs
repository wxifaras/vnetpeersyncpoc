using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Text;
using System.Text.Json;
using VNETPeeringSyncPoc;

public class VNetPeer : IVNetPeer
{
    private readonly ILogger<VNetPeer> _log;

    public VNetPeer(ILogger<VNetPeer> logger)
    {
        _log = logger;
    }

    public async Task<List<dynamic>> SyncVnetPeers(List<VNetSettings> settings)
    {
        var response = new List<dynamic>();

        if (settings.Any())
        {
            foreach (var setting in settings)
            {
                try
                {
                    if (setting.IsValid())
                    {
                        var settingJson = JsonSerializer.Serialize(setting);

                        _log.LogInformation($"Syncing peering {settingJson}");

                        var cred = new DefaultAzureCredential();
                        var client = new ArmClient(cred);

                        var virtualNetworkResourceId = VirtualNetworkResource.CreateResourceIdentifier(setting.SubscriptionOne, setting.ResourceGroupOne, setting.Vnet1);
                        var virtualNetwork = client.GetVirtualNetworkResource(virtualNetworkResourceId);

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

                        _log.LogInformation($"Peer syncing: {collection.Id}");

                        var syncRemoteAddressSpace = SyncRemoteAddressSpace.True;
                        ArmOperation<VirtualNetworkPeeringResource> lro = await collection.CreateOrUpdateAsync(WaitUntil.Completed, setting.VnetPeerName, data, syncRemoteAddressSpace: syncRemoteAddressSpace);
                        VirtualNetworkPeeringResource result = lro.Value;
                        VirtualNetworkPeeringData resourceData = result.Data;

                        _log.LogInformation($"Succeeded on id: {resourceData.Id}");

                        var json = JsonSerializer.Serialize(resourceData);

                        _log.LogInformation($"Response Result: {json}");

                        response.Add(resourceData);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Failed to sync peering");
                }
            }
        }

        return response;
    }

    public async Task<List<dynamic>> SyncVnetPeersHttp(List<VNetSettings> settings)
    {
        var response = new List<dynamic>();

        foreach (var setting in settings)
        {
            try
            {
                if (setting.IsValid())
                {
                    var settingJson = JsonSerializer.Serialize(setting);

                    _log.LogInformation($"Syncing peering {settingJson}");

                    var cred = new DefaultAzureCredential();
                    var context = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
                    var token = await cred.GetTokenAsync(context);

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");

                    var requestUri = $"https://management.azure.com/subscriptions/{setting.SubscriptionOne}/resourceGroups/{setting.ResourceGroupOne}/providers/Microsoft.Network/virtualNetworks/{setting.Vnet1}/virtualNetworkPeerings/{setting.VnetPeerName}?syncRemoteAddressSpace=true&api-version=2023-09-01";
                    var payload = new
                    {
                        properties = new
                        {
                            allowVirtualNetworkAccess = true,
                            allowForwardedTraffic = true,
                            allowGatewayTransit = false,
                            useRemoteGateways = false,
                            peerCompleteVnets = true,
                            remoteVirtualNetwork = new
                            {
                                id = $"/subscriptions/{setting.SubscriptionTwo}/resourceGroups/{setting.ResourceGroupTwo}/providers/Microsoft.Network/virtualNetworks/{setting.Vnet2}"
                            }
                        }
                    };

                    var jsonPayload = JsonSerializer.Serialize(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var httpResponse = await httpClient.PutAsync(requestUri, content);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        _log.LogInformation("Request succeeded.");

                        var responseBody = await httpResponse.Content.ReadAsStringAsync();

                        _log.LogInformation($"Response Body:{responseBody}");

                        response.Add(responseBody);
                    }
                    else
                    {
                        _log.LogError($"Request failed with status code: {httpResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to sync peering");
            }
        }
    
        return response;
    }

    //private async Task<TokenCredential> GetCredential()
    //{
    //    var authority = "https://login.microsoftonline.com/<tenantid>";
    //    var clientId = "";
    //    var clientSecret = "";

    //    string[] scopes = new string[] { "https://management.azure.com/.default" };

    //    var app = ConfidentialClientApplicationBuilder.Create(clientId)
    //        .WithClientSecret(clientSecret)
    //        .WithAuthority(new Uri(authority))
    //        .Build();

    //    var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

    //    var accessToken = result.AccessToken;
    //    var credential = new VNETPeerSyncPoc.SimpleTokenCredential(accessToken);
    //    return credential;
    //}
}

public interface IVNetPeer
{
    Task<List<dynamic>> SyncVnetPeers(List<VNetSettings> settings);
    Task<List<dynamic>> SyncVnetPeersHttp(List<VNetSettings> settings);
}