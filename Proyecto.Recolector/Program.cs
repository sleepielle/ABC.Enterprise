using Proyecto.Recolector.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Recolector>();
builder.Services.AddScoped<Recolector>();
builder.Services.AddControllers();
var app = builder.Build();

app.MapGet("/", () => "Recolector");

Recolector rec = new Recolector();

rec.SendSales();
app.Run();
