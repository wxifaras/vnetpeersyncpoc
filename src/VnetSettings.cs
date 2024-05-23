namespace VNETPeeringSyncPoc
{
    public  class VnetSettings
    {
        public required string SubscriptionId { get; set; }
        public required string ResourceGroup { get; set; }
        public required string Vnet1 { get; set; }
        public required string Vnet2 { get; set; }
        public required string VnetPeerName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SubscriptionId) &&
                   !string.IsNullOrEmpty(ResourceGroup) &&
                   !string.IsNullOrEmpty(Vnet1) &&
                   !string.IsNullOrEmpty(Vnet2) &&
                   !string.IsNullOrEmpty(VnetPeerName);
        }
    }
}