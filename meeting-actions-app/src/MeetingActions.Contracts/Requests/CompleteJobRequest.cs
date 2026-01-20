namespace MeetingActions.Contracts.Requests;

public record CompleteJobRequest
{
    public required string ResultJson { get; init; }
}
