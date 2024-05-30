namespace VNETPeeringSyncPoc
{
    public  class VNetSettings
    {
        public string SubscriptionOne { get; set; }
        public string SubscriptionTwo { get; set; }
        public string ResourceGroupOne { get; set; }
        public string ResourceGroupTwo { get; set; }
        public string Vnet1 { get; set; }
        public string Vnet2 { get; set; }
        public string VnetPeerName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SubscriptionOne) &&
                   !string.IsNullOrEmpty(SubscriptionTwo) &&
                   !string.IsNullOrEmpty(ResourceGroupOne) &&
                   !string.IsNullOrEmpty(ResourceGroupTwo) &&
                   !string.IsNullOrEmpty(Vnet1) &&
                   !string.IsNullOrEmpty(Vnet2) &&
                   !string.IsNullOrEmpty(VnetPeerName);
        }
    }
}