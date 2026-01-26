using MeetingActions.Api.Data;
using MeetingActions.Api.Entities;
using MeetingActions.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace MeetingActions.Worker.Services;

public class JobProcessor
{
    private readonly IDbContextFactory<MeetingActionsDbContext> _contextFactory;
    private readonly AzureOpenAIClient _openAIClient;
    private readonly ILogger<JobProcessor> _logger;

    public JobProcessor(
        IDbContextFactory<MeetingActionsDbContext> contextFactory,
        AzureOpenAIClient openAIClient,
        ILogger<JobProcessor> logger)
    {
        _contextFactory = contextFactory;
        _openAIClient = openAIClient;
        _logger = logger;
    }

    public async Task ProcessNextJobAsync(CancellationToken cancellationToken)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Find one pending job with InputType=Text
        var job = await db.Jobs
            .Where(j => j.Status == JobStatus.Pending && j.InputType == InputType.Text)
            .OrderBy(j => j.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (job == null)
        {
            // No pending jobs
            return;
        }

        _logger.LogInformation("Processing job {JobId}", job.Id);

        try
        {
            // Update status to Processing
            job.Status = JobStatus.Processing;
            job.UpdatedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Job {JobId} status updated to Processing", job.Id);

            // Call Azure OpenAI
            var resultJson = await _openAIClient.AnalyzeMeetingTranscriptAsync(
                job.TranscriptText ?? string.Empty,
                cancellationToken);

            _logger.LogInformation("Job {JobId} received result from Azure OpenAI", job.Id);

            // Save result
            var jobResult = new JobResult
            {
                JobId = job.Id,
                ResultJson = resultJson,
                CreatedAtUtc = DateTime.UtcNow
            };

            db.JobResults.Add(jobResult);

            // Update job status to Done
            job.Status = JobStatus.Done;
            job.UpdatedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Job {JobId} completed successfully", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobId} failed: {Error}", job.Id, ex.Message);

            // Update status to Failed
            job.Status = JobStatus.Failed;
            job.ErrorMessage = ex.Message.Length > 500 ? ex.Message.Substring(0, 500) : ex.Message;
            job.UpdatedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
