using MeetingActions.Contracts.Enums;

namespace MeetingActions.Contracts.Requests;

public record CreateJobRequest
{
    public required string MeetingType { get; init; }
    public required InputType InputType { get; init; }
    public string? TranscriptText { get; init; }
}
