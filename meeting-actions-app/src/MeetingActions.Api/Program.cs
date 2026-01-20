using MeetingActions.Api.Data;
using MeetingActions.Api.Entities;
using MeetingActions.Api.Validators;
using MeetingActions.Contracts.Enums;
using MeetingActions.Contracts.Requests;
using MeetingActions.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MeetingActionsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

// Endpoints
var v1 = app.MapGroup("/v1");

// POST /v1/jobs - Create job
v1.MapPost("/jobs", async (CreateJobRequest request, MeetingActionsDbContext db) =>
{
    var (isValid, errorMessage) = CreateJobRequestValidator.Validate(request);
    if (!isValid)
    {
        return Results.BadRequest(new { error = errorMessage });
    }

    var job = new Job
    {
        Id = Guid.NewGuid(),
        MeetingType = request.MeetingType,
        InputType = request.InputType,
        Status = JobStatus.Pending,
        TranscriptText = request.TranscriptText,
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };

    db.Jobs.Add(job);
    await db.SaveChangesAsync();

    return Results.Created($"/v1/jobs/{job.Id}", new CreateJobResponse { JobId = job.Id });
})
.WithName("CreateJob");

// GET /v1/jobs/{jobId} - Get job status
v1.MapGet("/jobs/{jobId:guid}", async (Guid jobId, MeetingActionsDbContext db) =>
{
    var job = await db.Jobs.FindAsync(jobId);
    
    if (job is null)
    {
        return Results.NotFound(new { error = "Job not found" });
    }

    var response = new JobStatusResponse
    {
        JobId = job.Id,
        Status = job.Status,
        CreatedAtUtc = job.CreatedAtUtc,
        UpdatedAtUtc = job.UpdatedAtUtc,
        ErrorMessage = job.ErrorMessage
    };

    return Results.Ok(response);
})
.WithName("GetJobStatus");

// GET /v1/jobs/{jobId}/result - Get job result
v1.MapGet("/jobs/{jobId:guid}/result", async (Guid jobId, MeetingActionsDbContext db) =>
{
    var job = await db.Jobs
        .Include(j => j.Result)
        .FirstOrDefaultAsync(j => j.Id == jobId);

    if (job is null)
    {
        return Results.NotFound(new { error = "Job not found" });
    }

    if (job.Status != JobStatus.Done)
    {
        return Results.Conflict(new { error = "Job is not completed yet", status = job.Status.ToString() });
    }

    if (job.Result is null)
    {
        return Results.NotFound(new { error = "Result not found" });
    }

    var response = new JobResultResponse
    {
        JobId = job.Id,
        ResultJson = job.Result.ResultJson
    };

    return Results.Ok(response);
})
.WithName("GetJobResult");

// POST /v1/jobs/{jobId}/_dev/complete - Dev endpoint to complete job
v1.MapPost("/jobs/{jobId:guid}/_dev/complete", async (Guid jobId, CompleteJobRequest request, MeetingActionsDbContext db) =>
{
    var job = await db.Jobs
        .Include(j => j.Result)
        .FirstOrDefaultAsync(j => j.Id == jobId);

    if (job is null)
    {
        return Results.NotFound(new { error = "Job not found" });
    }

    if (job.Result is not null)
    {
        return Results.Conflict(new { error = "Job already has a result" });
    }

    job.Status = JobStatus.Done;
    job.UpdatedAtUtc = DateTime.UtcNow;

    var result = new JobResult
    {
        JobId = jobId,
        ResultJson = request.ResultJson,
        CreatedAtUtc = DateTime.UtcNow
    };

    db.JobResults.Add(result);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Job completed successfully" });
})
.WithName("CompleteJobDev")
.WithTags("Development");

app.Run();
