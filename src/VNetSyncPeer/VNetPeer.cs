using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VNETPeeringSyncPoc;
using VNetSyncPeer;

public class VNetPeer : IVNetPeer
{
    private readonly ILogger<VNetPeer> _log;

    public VNetPeer(ILogger<VNetPeer> logger)
    {
        _log = logger;
    }

    public async Task<string> GetVnetPeer(VNetGetPeerRequest request)
    {
        var getVnetPeerResult = string.Empty;
        var cred = new DefaultAzureCredential();
        var client = new ArmClient(cred);

        ResourceIdentifier virtualNetworkResourceId = VirtualNetworkResource.CreateResourceIdentifier(request.SubscriptionId, request.ResourceGroup, request.VNetName);

        VirtualNetworkResource virtualNetwork = client.GetVirtualNetworkResource(virtualNetworkResourceId);

        VirtualNetworkPeeringCollection collection = virtualNetwork.GetVirtualNetworkPeerings();

        NullableResponse<VirtualNetworkPeeringResource> response = await collection.GetIfExistsAsync(request.VNetPeerName);

        VirtualNetworkPeeringResource? result = response.HasValue ? response.Value : null;

        if (result != null)
        {
            getVnetPeerResult = response.GetRawResponse().Content.ToString();
            _log.LogInformation($"Response Result: {getVnetPeerResult}");
        }
        else
        {
            _log.LogInformation("No peering found");
        }

        return getVnetPeerResult;
    }

    public async Task<string> SyncVnetPeers(VNetSyncPeeringRequest request)
    {
        var syncPeerResponse = string.Empty;

        if (request.IsValid())
        {
            var settingJson = JsonSerializer.Serialize(request);

            _log.LogInformation($"Syncing peering {settingJson}");

            var cred = new DefaultAzureCredential();
            var client = new ArmClient(cred);

            var virtualNetworkResourceId = VirtualNetworkResource.CreateResourceIdentifier(request.SubscriptionOne, request.ResourceGroupOne, request.Vnet1);
            
            var virtualNetwork = client.GetVirtualNetworkResource(virtualNetworkResourceId);

            var collection = virtualNetwork.GetVirtualNetworkPeerings();

            // invoke the operation
            var data = new VirtualNetworkPeeringData()
            {
                AllowVirtualNetworkAccess = true,
                AllowForwardedTraffic = true,
                AllowGatewayTransit = false,
                UseRemoteGateways = false,
                RemoteVirtualNetworkId = new ResourceIdentifier($"/subscriptions/{request.SubscriptionTwo}/resourceGroups/{request.ResourceGroupTwo}/providers/Microsoft.Network/virtualNetworks/{request.Vnet2}"),
            };

            _log.LogInformation($"Peer syncing: {collection.Id}");

            var syncRemoteAddressSpace = SyncRemoteAddressSpace.True;
            
            ArmOperation<VirtualNetworkPeeringResource> response = await collection.CreateOrUpdateAsync(WaitUntil.Completed, request.VnetPeerName, data, syncRemoteAddressSpace: syncRemoteAddressSpace);

            VirtualNetworkPeeringResource? result = response.HasValue ? response.Value : null;

            if (result != null)
            {
               _log.LogInformation($"Succeeded on id: {response.Value.Data.Id}");

               var temp = await response.Value.GetAsync();

               syncPeerResponse = temp.GetRawResponse().Content.ToString();

               _log.LogInformation($"Response Result: {syncPeerResponse}");
            }
            else
            {
                _log.LogInformation("Unable to sync peers.");
            }
        }

        return syncPeerResponse;
    }
}