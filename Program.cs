using Microsoft.AspNetCore.HttpOverrides;
using Plataforma24API.Extensions;
using Plataforma24API.Infrastructure.Serialization;
using System.Net;

// Carga .env antes de crear el builder para que sus variables entren en la configuracion.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opciones => opciones.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppAuthorization();
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddGraphQLApi();
builder.Services.AddCors(opciones => opciones.AddDefaultPolicy(politica =>
    politica.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Detras del proxy inverso: IP real del cliente (rate limit por IP) y esquema original (redireccion a HTTPS).
// Por defecto solo se confia en un proxy en la misma maquina; otros se agregan en Proxy__IpsConfiables__0, __1...
builder.Services.Configure<ForwardedHeadersOptions>(opciones =>
{
    opciones.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var ip in builder.Configuration.GetSection("Proxy:IpsConfiables").Get<string[]>() ?? [])
        opciones.KnownProxies.Add(IPAddress.Parse(ip));
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
