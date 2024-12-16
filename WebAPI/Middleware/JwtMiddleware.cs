using System.IdentityModel.Tokens.Jwt;
using Application.Interfaces;

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

                        if (jti == null)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        // Validar Access Token en la base de datos
                        var isAccessTokenValid = await authenticationService.ValidateToken(token);

                        if (isAccessTokenValid)
                        {
                            // Access Token válido: continuar con la solicitud
                            await _next(context);
                            return;
                        }

                        // Si el Access Token no es válido, intentamos validar como Refresh Token
                        if (IsRefreshToken(jwtToken))
                        {
                            // Permitir acceso al endpoint de refresco
                            if (context.Request.Path.StartsWithSegments("/api/auth/refresh-token"))
                            {
                                await _next(context);
                                return;
                            }
                        }

                        // Ningún token válido
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
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

        private bool IsRefreshToken(JwtSecurityToken jwtToken)
        {
            // Verifica si el token aún está dentro del tiempo de expiración (120 minutos)
            var expiration = jwtToken.ValidTo;
            return expiration > DateTime.UtcNow;
        }

    }

}
