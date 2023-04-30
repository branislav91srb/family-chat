using ChatServer.Services.Abstraction;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChatServer.Auth
{
    public class IdAuthSchemeHandler : AuthenticationHandler<IdAuthSchemeOptions>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IdAuthSchemeHandler> _logger;

        public IdAuthSchemeHandler(
            IOptionsMonitor<IdAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserRepository userRepository)
            : base(options, logger, encoder, clock)
        {
            _userRepository = userRepository;
            _logger = logger.CreateLogger<IdAuthSchemeHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            long userId = GetUserId(Request);

            if(userId == 0)
            {
                return AuthenticateResult.Fail("No auth header.");
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                var claimsIdentity = new ClaimsIdentity("Chat");
                claimsIdentity.AddClaim(new Claim("Id", userId.ToString()));
                var principal = new ClaimsPrincipal(claimsIdentity);

                var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Auth error!", ex);
                return AuthenticateResult.Fail(ex);
            }
        }

        private long GetUserId(HttpRequest request)
        {
            long userId = 0;
            string userIdString = string.Empty;

            // For API
            var authHeader = Request.Headers.Authorization.FirstOrDefault();

            // For SignalR
            var authRequest = Request.Query["access_token"].ToString();

            if (authHeader != null)
            {
                userIdString = authHeader.Replace("Bearer ", "");
                
            }else if(authRequest != null)
            {
                userIdString = authRequest.Replace("Bearer ", "");
            }

            var success = long.TryParse(userIdString, out long result);

            if (success)
            {
                userId = result;
            }

            return userId;
        }
    }
}
