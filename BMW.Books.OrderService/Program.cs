using BMW.Books.OrderService.Services;
using BMW.Books.OrderService.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Configuration
var bookServiceBase = builder.Configuration["BOOKS_BASE_URL"] ?? "http://book-catalogue:8080";
var auditingHost = builder.Configuration["AUDIT_HOST"] ?? "auditing-service";
var auditingPort = int.TryParse(builder.Configuration["AUDIT_PORT"], out var p) ? p : 5140;

builder.Services.AddHttpClient("books", c => c.BaseAddress = new Uri(bookServiceBase));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

OrderEndpoints.MapOrderEndpoints(app);
HealthEndpoints.MapHealthEndpoints(app);

app.Run();