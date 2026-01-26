using MeetingActions.Api.Data;
using MeetingActions.Worker;
using MeetingActions.Worker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Add DbContext Factory (for creating context per iteration)
builder.Services.AddDbContextFactory<MeetingActionsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpClient for Azure OpenAI (singleton, reusable)
builder.Services.AddHttpClient<AzureOpenAIClient>();

// Add services
builder.Services.AddSingleton<JobProcessor>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
