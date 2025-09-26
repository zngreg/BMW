using BMW.AuditingService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAuditListener, AuditListener>();


var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

var auditListener = app.Services.GetRequiredService<IAuditListener>();
var cts = new CancellationTokenSource();
_ = Task.Run(() => auditListener.StartAsync(cts.Token));

app.Lifetime.ApplicationStopping.Register(() => cts.Cancel());
app.Run();