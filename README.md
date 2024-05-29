# Overview
This is a simple Proof of Concept (POC) console application that demonstrates how to perform Sync Peering between two virtual networks in Azure using the Azure .NET SDK. The code assumes <b>Hub-spoke</b> network topology.

In Azure, “sync peering” refers to the process of updating the address space of a peered virtual network.
When you update the address space for a virtual network, you need to sync the virtual network peer for each remote peered virtual network.

## Configuration
The POC uses a configuration file to set up Azure Virtual Network (VNet) peering. The configuration is stored in a `VNETSettings` array in the `appsettings.json` file. Each object in the array represents a different VNet peering configuration.

Here's what each property means:

- `SubscriptionOne`: The ID of your Hub Azure subscription.
- `SubscriptionTwo`: The ID of your Spoke Azure subscription.
- `ResourceGroupOne`: The name of the resource group where your Hub VNET is located.
- `ResourceGroupTwo`: The name of the resource group where your Spoke VNET is located.
- `Vnet1`: The name of the Hub VNet.
- `Vnet2`: The name of the Spoke VNet.
- `VnetPeerName`: The name of the Hub VNet peer.

## Authentication
The POC uses the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?tabs=command-line) to authenticate.

## Reference Documenation

[Sync Peering](https://learn.microsoft.com/en-us/rest/api/virtualnetwork/virtual-network-peerings/create-or-update?view=rest-virtualnetwork-2023-09-01&tabs=dotnet#sync-peering)
