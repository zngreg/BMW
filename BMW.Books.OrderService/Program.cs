using BMW.Books.OrderService.Services;
using BMW.Books.OrderService.Endpoints;
using BMW.Books.OrderService.Helpers;
using BMW.Books.OrderService.Middlewares;
using BMW.Books.OrderService.Clients;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var bookServiceBase = builder.Configuration["BOOKS_BASE_URL"] ?? "http://book-catalogue:8080";

builder.Services.AddHttpClient("books", c => c.BaseAddress = new Uri(bookServiceBase));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UdpAuditService>();
builder.Services.AddScoped<RabbitMqAuditService>();
builder.Services.AddScoped<IStockUpdateService, RabbitMqStockUpdateService>();
builder.Services.AddScoped<IAuditServiceFactory, AuditServiceFactory>();
builder.Services.AddScoped<IBookCatalogClient, BookCatalogClient>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Services.GetRequiredService<IAuditServiceFactory>();

// Endpoints
OrderEndpoints.MapOrderEndpoints(app, ValidationHelper.Validate);
HealthEndpoints.MapHealthEndpoints(app);

// Global error handler
var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.UseGlobalErrorHandler(logger);

app.Run();