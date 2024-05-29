using Azure.Core;

namespace VNETPeerSyncPoc
{
    public class SimpleTokenCredential : TokenCredential
    {
        private string _token;

        public SimpleTokenCredential(string token)
        {
            _token = token;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(_token, DateTimeOffset.Now.AddDays(1));
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(Task.FromResult(new AccessToken(_token, DateTimeOffset.Now.AddDays(1))));
        }
    }
}