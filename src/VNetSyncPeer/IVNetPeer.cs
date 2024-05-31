using VNETPeeringSyncPoc;

namespace VNetSyncPeer
{
    public interface IVNetPeer
    {
        Task<string> SyncVnetPeers(VNetSyncPeeringRequest request);
        Task<string> GetVnetPeer(VNetGetPeerRequest request);
    }
}
