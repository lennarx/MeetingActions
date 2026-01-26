using MeetingActions.Worker.Services;

namespace MeetingActions.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly JobProcessor _jobProcessor;
    private readonly int _pollingIntervalSeconds;

    public Worker(
        ILogger<Worker> logger,
        JobProcessor jobProcessor,
        IConfiguration configuration)
    {
        _logger = logger;
        _jobProcessor = jobProcessor;
        _pollingIntervalSeconds = configuration.GetValue<int>("Worker:PollingIntervalSeconds", 2);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Meeting Actions Worker started at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _jobProcessor.ProcessNextJobAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in worker loop");
            }

            await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Meeting Actions Worker stopping at: {Time}", DateTimeOffset.Now);
    }
}
