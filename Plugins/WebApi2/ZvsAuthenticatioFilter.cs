using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace zvsWebapi2Plugin
{
    public class ZvsAuthenticatioFilter : IAuthenticationFilter
    {
        private readonly WebApi2Plugin _webApi2Plugin;
        public ZvsAuthenticatioFilter(WebApi2Plugin webApi2Plugin)
        {
            if (webApi2Plugin == null)
                throw new ArgumentNullException("webApi2Plugin");

            _webApi2Plugin = webApi2Plugin;
        }
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var token = string.Empty;

            IEnumerable<String> values;
            if (context.Request.Headers.TryGetValues("X-zvsToken", out values))
                token = values.FirstOrDefault();

            var authorizedTokens = new List<string>(_webApi2Plugin.TokensSettings.Split(','));

            if (token == null || !authorizedTokens.Any(o => o.Trim().Equals(token)))
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "ZvsApi User"),
                    new Claim(ClaimTypes.Role, "All Access")
                };
            var id = new ClaimsIdentity(claims, "StaticToken");
            var principal = new ClaimsPrincipal(new[] { id });
            context.Principal = principal;

            return Task.FromResult(0);
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
          //  context.Result = new ResponseMessageResult(await context.Result.ExecuteAsync(cancellationToken));
        }

        public bool AllowMultiple
        {
            get { throw new NotImplementedException(); }
        }
    }
}
