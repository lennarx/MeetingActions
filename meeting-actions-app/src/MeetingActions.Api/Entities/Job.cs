using MeetingActions.Contracts.Enums;

namespace MeetingActions.Api.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string MeetingType { get; set; } = string.Empty;
    public InputType InputType { get; set; }
    public JobStatus Status { get; set; }
    public string? TranscriptText { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation
    public JobResult? Result { get; set; }
}
