using BMW.Books.OrderService.Services;
using BMW.Books.OrderService.Endpoints;
using Microsoft.AspNetCore.RateLimiting;


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

// CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Rate limiting
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = 429;
    opts.AddFixedWindowLimiter("general", o => { o.PermitLimit = 100; o.Window = TimeSpan.FromMinutes(1); o.QueueLimit = 0; });
});

app.UseSwagger();
app.UseSwaggerUI();

OrderEndpoints.MapOrderEndpoints(app);
HealthEndpoints.MapHealthEndpoints(app);

app.Run();