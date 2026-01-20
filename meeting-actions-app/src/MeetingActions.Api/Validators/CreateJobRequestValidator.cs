using MeetingActions.Contracts.Enums;
using MeetingActions.Contracts.Requests;

namespace MeetingActions.Api.Validators;

public static class CreateJobRequestValidator
{
    public static (bool IsValid, string? ErrorMessage) Validate(CreateJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MeetingType))
        {
            return (false, "meetingType is required");
        }

        if (request.MeetingType.Length > 30)
        {
            return (false, "meetingType must be 30 characters or less");
        }

        if (!Enum.IsDefined(typeof(InputType), request.InputType))
        {
            return (false, "inputType is invalid");
        }

        if (request.InputType == InputType.Text)
        {
            if (string.IsNullOrWhiteSpace(request.TranscriptText))
            {
                return (false, "transcriptText is required when inputType is Text");
            }

            if (request.TranscriptText.Length < 10)
            {
                return (false, "transcriptText must be at least 10 characters");
            }
        }

        return (true, null);
    }
}
