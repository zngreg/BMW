using BMW.Books.OrderService.Services;
using BMW.Books.OrderService.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Configuration
var bookServiceBase = builder.Configuration["BOOKS_BASE_URL"] ?? "http://book-catalogue:8080";

builder.Services.AddHttpClient("books", c => c.BaseAddress = new Uri(bookServiceBase));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UdpAuditService>();
builder.Services.AddScoped<RabbitMqAuditService>();
builder.Services.AddScoped<IAuditServiceFactory, AuditServiceFactory>();
builder.Services.AddSingleton<IOrderService, OrderService>();


var app = builder.Build();

app.Services.GetRequiredService<IAuditServiceFactory>();

app.UseSwagger();
app.UseSwaggerUI();

OrderEndpoints.MapOrderEndpoints(app);
HealthEndpoints.MapHealthEndpoints(app);

app.Run();