namespace MeetingActions.Api.Entities;

public class JobResult
{
    public Guid JobId { get; set; }
    public string ResultJson { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    // Navigation
    public Job Job { get; set; } = null!;
}
