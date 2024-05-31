namespace VNetSyncPeer
{
    public class VNetGetPeerRequest
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string VNetName { get; set; }
        public string VNetPeerName { get; set; }
    }
}
