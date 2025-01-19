using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            bool loginPath = context.Request.Path.StartsWithSegments("/api/auth/login");

            if (token != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

                    var validatedToken = await ValidateToken(context, token, authenticationService);
                    if (!validatedToken)
                    {
                        return;
                    }

                    bool refreshPath = context.Request.Path.StartsWithSegments("/api/auth/refresh-token");

                    var tokenType = context.User.FindFirst("TokenType")?.Value;

                    if ((tokenType == TokenType.Access.ToString() && !refreshPath && !loginPath) ||
                        (tokenType == TokenType.Refresh.ToString() && refreshPath))
                    {
                        await _next(context);

                        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                            await WriteErrorResponse(context, StatusCodes.Status403Forbidden, "El usuario no tiene permisos.");
                        return;
                    }

                    await WriteErrorResponse(context, StatusCodes.Status403Forbidden, "El token no tiene permisos.");
                    return;
                }
            }

            if (loginPath)
            {
                await _next(context);
                return;
            }

            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token inválido o sesión expirada.");
        }


        private async Task<bool> ValidateToken(HttpContext context, string token, IAuthenticationService authenticationService)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "TokenType")?.Value;

                if (jti == null || string.IsNullOrEmpty(tokenType))
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token inválido o sesión expirada.");
                    return false;
                }

                bool isTokenValid = await authenticationService.ValidateToken(token);
                if (!isTokenValid)
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token inválido o sesión expirada.");
                    return false;
                }

                var claims = jwtToken.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();

                var identity = new ClaimsIdentity(claims, "Jwt");
                context.User = new ClaimsPrincipal(identity);
                if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token inválido o no proporcionado.");
                    return false;
                }

                return true;
            }
            catch
            {
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token inválido o sesión expirada.");
                return false;
            }
        }


        private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }


    }

}
