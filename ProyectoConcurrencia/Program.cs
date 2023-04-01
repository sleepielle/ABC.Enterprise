
using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Proyecto.Validaciones.Services.ReceiveMessages>();
builder.Services.AddScoped<Proyecto.Validaciones.Services.ReceiveMessages>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Validaciones");
app.MapControllers();
app.Run();

