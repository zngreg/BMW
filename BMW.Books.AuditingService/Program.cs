using BMW.Books.AuditingService.Endpoints;
using BMW.Books.AuditingService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UdpAuditListener>();
builder.Services.AddSingleton<RabbitMqAuditListener>();
builder.Services.AddSingleton<IAuditListenerFactory, AuditListenerFactory>();

var app = builder.Build();

var factory = app.Services.GetRequiredService<IAuditListenerFactory>();
var cts = new CancellationTokenSource();
// var listener = factory.CreateListener(app.Services);
// _ = Task.Run(() => listener.StartAsync(cts.Token));

var udpAuditListener = app.Services.GetRequiredService<UdpAuditListener>();
_ = Task.Run(() => udpAuditListener.StartAsync(cts.Token));

var rabbitMqAuditListener = app.Services.GetRequiredService<RabbitMqAuditListener>();
_ = Task.Run(() => rabbitMqAuditListener.StartAsync(cts.Token));

app.Lifetime.ApplicationStopping.Register(() => cts.Cancel());

HealthEndpoints.MapHealthEndpoints(app);

app.Run();