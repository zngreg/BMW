using BMW.Books.CatalogueService.Services;
using BMW.Books.CatalogueService.Endpoints;
using BMW.Books.CatalogueService.Middleware;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UdpAuditService>();
builder.Services.AddScoped<RabbitMqAuditService>();
builder.Services.AddScoped<IAuditServiceFactory, AuditServiceFactory>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// app.UseRateLimiter();
app.UseGlobalErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

BookEndpoints.MapBookEndpoints(app);
HealthEndpoints.MapHealthEndpoints(app);

app.Run();