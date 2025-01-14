using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using WebAPI.Extensions;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddSwaggerWithJwtSupport();
builder.Services.AddCorsConfiguration();


builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowSpecificOrigin"); // Aplica la política de CORS

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
