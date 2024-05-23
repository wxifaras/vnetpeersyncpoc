using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using VNETPeeringSyncPoc;

public class VnetPeerSync
{
    public async Task SyncPeering(List<VnetSettings> vnetSettings)
    {
        foreach(var setting in vnetSettings)
        {
            if (setting.IsValid())
            {
                // get your azure access token, for more details of how Azure SDK get your access token, please refer to https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication?tabs=command-line
                var cred = new DefaultAzureCredential();
                // authenticate your client
                var client = new ArmClient(cred);

                var virtualNetworkResourceId = VirtualNetworkResource.CreateResourceIdentifier(setting.SubscriptionId, setting.ResourceGroup, setting.Vnet1);
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
                    RemoteVirtualNetworkId = new ResourceIdentifier($"/subscriptions/{setting.SubscriptionId}/resourceGroups/{setting.ResourceGroup}/providers/Microsoft.Network/virtualNetworks/{setting.Vnet2}"),
                };

                Console.WriteLine($"Peer syncing: {collection.Id}");

                var syncRemoteAddressSpace = SyncRemoteAddressSpace.True;
                ArmOperation<VirtualNetworkPeeringResource> lro = await collection.CreateOrUpdateAsync(WaitUntil.Completed, setting.VnetPeerName, data, syncRemoteAddressSpace: syncRemoteAddressSpace);
                VirtualNetworkPeeringResource result = lro.Value;

                // the variable result is a resource, you could call other operations on this instance as well
                // but just for demo, we get its data from this resource instance
                VirtualNetworkPeeringData resourceData = result.Data;
                Console.WriteLine($"Succeeded on id: {resourceData.Id}");
            }
            else
            {
                Console.WriteLine("Invalid VnetSettings");
            }
        }
    }
}