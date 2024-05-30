using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VNETPeeringSyncPoc;

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

        [Function("VNetPeerFunction")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("VNetPeerFunction Invoked");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            _logger.LogInformation($"Request: {requestBody}");

            var settings = JsonSerializer.Deserialize<List<VNetSettings>>(requestBody);
                        
            var response = await _peer.SyncVnetPeersHttp(settings);

            var jsonResult = JsonSerializer.Serialize(response);

            _logger.LogInformation($"Response: {jsonResult}");

            return new OkObjectResult(jsonResult);
        }
    }
}