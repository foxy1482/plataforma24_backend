using Plataforma24API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppAuthorization();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
