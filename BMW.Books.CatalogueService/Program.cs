using BMW.Books.CatalogueService.Services;
using BMW.Books.CatalogueService.Endpoints;
using BMW.Books.CatalogueService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddGlobalRateLimiter();

var app = builder.Build();

// app.UseRateLimiter();
app.UseGlobalErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

BookEndpoints.MapBookEndpoints(app);
HealthEndpoints.MapHealthEndpoints(app);

app.Run();