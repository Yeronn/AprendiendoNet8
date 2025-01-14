using System.IdentityModel.Tokens.Jwt;
using Application.Interfaces;
using Domain.Enums;

namespace WebAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public JwtMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                    var tokenHandler = new JwtSecurityTokenHandler();

                    try
                    {
                        var jwtToken = tokenHandler.ReadJwtToken(token);
                        var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                        var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "TokenType")?.Value;

                        if (jti == null || string.IsNullOrEmpty(tokenType))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        bool isTokenValid = await authenticationService.ValidateToken(token);
                        if (!isTokenValid)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        bool refreshPath = context.Request.Path.StartsWithSegments("/api/auth/refresh-token");
                        
                        if (tokenType == TokenType.Access.ToString() && !refreshPath)
                        {
                            await _next(context);
                            return;
                        }

                        if (tokenType == TokenType.Refresh.ToString() && refreshPath)
                        {
                            await _next(context);
                            return;
                        }

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }
            await _next(context);
        }

    }

}
