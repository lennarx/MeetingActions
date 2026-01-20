namespace MeetingActions.Contracts.Responses;

public record CreateJobResponse
{
    public required Guid JobId { get; init; }
}
