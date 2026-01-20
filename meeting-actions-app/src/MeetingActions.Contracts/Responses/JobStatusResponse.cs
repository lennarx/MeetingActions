using MeetingActions.Contracts.Enums;

namespace MeetingActions.Contracts.Responses;

public record JobStatusResponse
{
    public required Guid JobId { get; init; }
    public required JobStatus Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
}
