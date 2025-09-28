using BMW.Books.CatalogueService.Services;
using BMW.Books.CatalogueService.Endpoints;
using BMW.Books.CatalogueService.Middlewares;
using BMW.Books.CatalogueService.Helpers;
using BMW.Books.CatalogueService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddScoped<UdpAuditService>();
builder.Services.AddScoped<RabbitMqAuditService>();
builder.Services.AddScoped<IAuditServiceFactory, AuditServiceFactory>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IStockUpdateListener, RabbitMqStockUpdateListener>();

var app = builder.Build();

var cts = new CancellationTokenSource();
var rabbitMqStockUpdateListener = app.Services.GetRequiredService<IStockUpdateListener>();
_ = Task.Run(() => rabbitMqStockUpdateListener.StartAsync(cts.Token));

app.Lifetime.ApplicationStopping.Register(() => cts.Cancel());

app.Services.GetRequiredService<IAuditServiceFactory>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Endpoints
BookEndpoints.MapBookEndpoints(app, ValidationHelper.Validate);
HealthEndpoints.MapHealthEndpoints(app);

// Global error handler
app.UseGlobalErrorHandler();

app.Run();