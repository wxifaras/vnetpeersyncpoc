using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VNETPeeringSyncPoc;
using VNetSyncPeer;

namespace SyncPeeringFunction
{
    public class VNetPeerFunction
    {
        private readonly IVNetPeer _peer;
        private readonly ILogger<VNetPeerFunction> _logger;

        public VNetPeerFunction(IVNetPeer peer, ILogger<VNetPeerFunction> logger)
        {
            _peer = peer;
            _logger = logger;
        }

        [Function("SyncVnetPeer")]
        public async Task<IActionResult> SyncVnetPeer([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("SyncVnetPeerFunction Invoked");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult("Request body is empty");
                }

                _logger.LogInformation($"Request: {requestBody}");

                var vnetSyncPeeringRequest = JsonSerializer.Deserialize<VNetSyncPeeringRequest>(requestBody);

                var response = await _peer.SyncVnetPeers(vnetSyncPeeringRequest);

                _logger.LogInformation($"Response: {response}");

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync peering");

                return new BadRequestObjectResult("Failed to sync peering");
            }
        }

        [Function("GetVnetPeer")]
        public async Task<IActionResult> GetVnetPeer([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("GetVnetPeer Invoked");

                var peeringData = string.Empty;

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult("Request body is empty");
                }

                _logger.LogInformation($"Request: {requestBody}");

                var getVnetPeerRequest = JsonSerializer.Deserialize<VNetGetPeerRequest>(requestBody);

                peeringData = await _peer.GetVnetPeer(getVnetPeerRequest);

                _logger.LogInformation($"Response: {peeringData}");

                return new OkObjectResult(peeringData);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get peering");

                return new BadRequestObjectResult("Failed to get peering");
            }
        }
    }
}