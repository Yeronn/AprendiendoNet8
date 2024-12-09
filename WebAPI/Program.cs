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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//TODO: El middleware que verifica que solo haya una sesion abierta no deja que funcione la autorizacion en los controladores
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
