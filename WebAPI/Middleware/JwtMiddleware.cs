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
                    var isValid = await authenticationService.ValidateToken(token);
                    if (isValid)
                    {
                        // Token válido, puedes agregar la lógica para el usuario aquí si es necesario
                    }
                    else
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
