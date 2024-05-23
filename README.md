# vnetpeersyncpoc
C# .NET Sync Peering Poc

## Overview
This is a simple POC that demonstrates how to perform Sync Peering between two virtual networks in Azure using the Azure .NET SDK.

In Azure, “sync peering” refers to the process of updating the address space of a peered virtual network.
When you update the address space for a virtual network, you need to sync the virtual network peer for each remote peered virtual network.

## Configuration
The POC uses a configuration file to set up Azure Virtual Network (VNet) peering. The configuration is stored in a `VNETSettings` array in the `appsettings.json` file. Each object in the array represents a different VNet peering configuration.

Here's what each property means:

- `SubscriptionId`: The ID of your Azure subscription.
- `ResourceGroup`: The name of the resource group where your VNets are located.
- `Vnet1`: The name of the first VNet in the peering.
- `Vnet2`: The name of the second VNet in the peering.
- `VnetPeerName`: The name of the VNet peering.

## Reference Documenation

[Sync Peering](https://learn.microsoft.com/en-us/rest/api/virtualnetwork/virtual-network-peerings/create-or-update?view=rest-virtualnetwork-2023-09-01&tabs=dotnet#sync-peering)
