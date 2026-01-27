using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using MeetingActions.Api.Data;
using MeetingActions.Worker;
using MeetingActions.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
    }
}

// Add DbContext Factory (for creating context per iteration)
builder.Services.AddDbContextFactory<MeetingActionsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpClient for Azure OpenAI (singleton, reusable)
builder.Services.AddHttpClient<AzureOpenAIClient>();

// Add services
builder.Services.AddSingleton<JobProcessor>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MeetingActionsDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await db.Database.MigrateAsync();
    }
}
host.Run();
