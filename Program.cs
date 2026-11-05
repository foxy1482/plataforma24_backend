using Plataforma24API.Extensions;
using Plataforma24API.Infrastructure.Serialization;

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

var app = builder.Build();

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
