# Overview
This is simple proof of concept (POC) code that demonstrates how to perform Sync Peering between two virtual networks in Azure using the Azure .NET SDK. The code assumes <b>Hub-spoke</b> network topology.

The sample code is shown in a Console Application and an Azure Function.

## Sync Peering

In Azure, “sync peering” refers to the process of updating the address space of a peered virtual network.
When you update the address space for a virtual network, you need to sync the virtual network peer for each remote peered virtual network.

<b>Sync peering has to run in the VNet that has "LocalNotInSync".</b>

## Configuration
### VNetPeerSyncConsoleApp
<b>VNETSettings</b>
- `SubscriptionOne`: subscription id
- `SubscriptionTwo`: subscription id
- `ResourceGroupOne`: resource group name
- `ResourceGroupTwo`: resource group name
- `Vnet1`: VNet name
- `Vnet2`: VNet name
- `VnetPeerName`: VNet peer name
  
### SyncPeeringFunction
<b>Request Body</b>
```json
[
    {
        "SubscriptionOne": "subscription id",
        "SubscriptionTwo": "subscription id",
        "ResourceGroupOne": "resource group name",
        "ResourceGroupTwo": "resource group name",
        "Vnet1": "VNet name",
        "Vnet2": "VNet name",
        "VnetPeerName": "VNet peer name"
    }
]
```
## Authentication
The POC uses the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?tabs=command-line) to authenticate.

## Reference Documenation

[Sync Peering](https://learn.microsoft.com/en-us/rest/api/virtualnetwork/virtual-network-peerings/create-or-update?view=rest-virtualnetwork-2023-09-01&tabs=dotnet#sync-peering)

[Virtual Network Peerings - Get](https://learn.microsoft.com/en-us/rest/api/virtualnetwork/virtual-network-peerings/get?view=rest-virtualnetwork-2023-09-01&tabs=HTTP)
