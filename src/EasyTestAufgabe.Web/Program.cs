using EasyTestAufgabe.Application.Abstractions;
using EasyTestAufgabe.Application.Services;
using EasyTestAufgabe.Infrastructure.Persistence;
using EasyTestAufgabe.Web.Components;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Serilog so früh wie möglich konfigurieren, damit auch
// Startup-Fehler (z.B. fehlerhafte Konfiguration) protokolliert werden. 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

try
{
    Log.Information("EasyTestAufgabe wird gestartet...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog als Logging-Provider verwenden 
    builder.Host.UseSerilog();

    // Blazor (aus dem Template, --interactivity Server) 
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // EF Core / SQLite
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=app.db";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));

    // Application-Services 
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<ITimeEntryService, TimeEntryService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    // ein Log-Eintrag pro HTTP-Request (Methode, Pfad, Status, Dauer) 
    app.UseSerilogRequestLogging();

    app.UseAntiforgery();
    app.MapStaticAssets();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    // Migration + Seed beim Start 
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db);
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException wird u.a. von "dotnet ef" beim Auslesen des Designtime-
    // Modells geworfen — das ist kein echter Fehler und soll nicht geloggt werden.
    Log.Fatal(ex, "Anwendung unerwartet beendet.");
}
finally
{
    Log.CloseAndFlush();
}