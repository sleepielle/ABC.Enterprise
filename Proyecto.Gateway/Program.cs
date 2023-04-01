
using Microsoft.AspNetCore.Mvc;
using Proyecto.Gateway.Controllers;
using Proyecto.Gateway.Services;
//using Proyecto.Concurrencia.RabbitMq;
//using Proyecto.Concurrencia.Validations;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Transaction>();
builder.Services.AddHttpClient<GatewayController>();
builder.Services.AddControllers();
var app = builder.Build();

app.MapGet("/", () => "Gateway");
app.MapControllers();
app.Run();
