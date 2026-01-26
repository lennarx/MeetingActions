using System.Text.Json.Serialization;

namespace MeetingActions.Worker.Models;

public record OpenAIChatRequest(
    List<OpenAIChatMessage> Messages,
    double Temperature = 0.2,
    [property: JsonPropertyName("max_tokens")] int MaxTokens = 4000
);

public record OpenAIChatMessage(
    string Role,
    string Content
);

public record OpenAIChatResponse(
    string Id,
    string Object,
    long Created,
    string Model,
    List<OpenAIChatChoice> Choices,
    OpenAIUsage Usage
);

public record OpenAIChatChoice(
    int Index,
    OpenAIChatMessage Message,
    string FinishReason
);

public record OpenAIUsage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
);

// Result structure matching frontend expectations
public record MeetingAnalysisResult(
    List<string>? Decisions,
    List<string>? Actions,
    List<string>? ImplicitDates,
    List<string>? Risks,
    List<string>? OpenQuestions
);
