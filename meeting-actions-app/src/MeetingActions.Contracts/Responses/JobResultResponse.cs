namespace MeetingActions.Contracts.Responses;

public record JobResultResponse
{
    public required Guid JobId { get; init; }
    public required string ResultJson { get; init; }
}
