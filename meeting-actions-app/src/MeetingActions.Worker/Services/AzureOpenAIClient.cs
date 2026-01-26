using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MeetingActions.Worker.Models;

namespace MeetingActions.Worker.Services;

public class AzureOpenAIClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AzureOpenAIClient> _logger;
    private readonly string _endpoint;
    private readonly string _deployment;
    private readonly string _apiVersion;

    public AzureOpenAIClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AzureOpenAIClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Read from environment variables, fallback to appsettings
        _endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                    ?? configuration["AzureOpenAI:Endpoint"]
                    ?? throw new InvalidOperationException("Azure OpenAI Endpoint not configured");

        _deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")
                      ?? configuration["AzureOpenAI:Deployment"]
                      ?? throw new InvalidOperationException("Azure OpenAI Deployment not configured");

        _apiVersion = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_VERSION")
                      ?? configuration["AzureOpenAI:ApiVersion"]
                      ?? "2024-02-15-preview";

        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
                     ?? configuration["AzureOpenAI:ApiKey"]
                     ?? throw new InvalidOperationException("Azure OpenAI API Key not configured");

        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> AnalyzeMeetingTranscriptAsync(string transcriptText, CancellationToken cancellationToken)
    {
        var systemPrompt = @"You are an expert meeting analyzer. Extract the following information from meeting transcripts:
1. Decisions: Key decisions made during the meeting
2. Actions: Action items with clear owners and deadlines if mentioned
3. ImplicitDates: Any dates, deadlines, or timeframes mentioned
4. Risks: Potential risks, blockers, or concerns raised
5. OpenQuestions: Unanswered questions or topics requiring follow-up

Return ONLY valid JSON with this exact structure:
{
  ""decisions"": [""decision 1"", ""decision 2""],
  ""actions"": [""action 1"", ""action 2""],
  ""implicitDates"": [""date 1""],
  ""risks"": [""risk 1""],
  ""openQuestions"": [""question 1""]
}";

        var userPrompt = $"Analyze this meeting transcript:\n\n{transcriptText}";

        var request = new OpenAIChatRequest(
            Messages: new List<OpenAIChatMessage>
            {
                new("system", systemPrompt),
                new("user", userPrompt)
            },
            Temperature: 0.2,
            MaxTokens: 4000
        );

        var url = $"{_endpoint}/openai/deployments/{_deployment}/chat/completions?api-version={_apiVersion}";

        _logger.LogInformation("Calling Azure OpenAI at {Url}", url);

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Azure OpenAI API error: {StatusCode} - {Error}",
                response.StatusCode, errorContent);
            throw new HttpRequestException($"Azure OpenAI API returned {response.StatusCode}: {errorContent}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var chatResponse = JsonSerializer.Deserialize<OpenAIChatResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (chatResponse?.Choices == null || chatResponse.Choices.Count == 0)
        {
            throw new InvalidOperationException("Azure OpenAI returned no choices");
        }

        var resultText = chatResponse.Choices[0].Message.Content;

        // Extract JSON if it's wrapped in markdown code blocks or extra text
        resultText = ExtractJsonFromResponse(resultText);

        // Validate it's valid JSON
        try
        {
            JsonDocument.Parse(resultText);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON from OpenAI response: {Response}", resultText);
            throw new InvalidOperationException("OpenAI returned invalid JSON", ex);
        }

        return resultText;
    }

    private string ExtractJsonFromResponse(string response)
    {
        // Remove markdown code blocks if present
        response = response.Trim();

        if (response.StartsWith("```json"))
        {
            response = response.Substring(7);
        }
        else if (response.StartsWith("```"))
        {
            response = response.Substring(3);
        }

        if (response.EndsWith("```"))
        {
            response = response.Substring(0, response.Length - 3);
        }

        response = response.Trim();

        // Find first { and last } to extract JSON object
        var startIndex = response.IndexOf('{');
        var endIndex = response.LastIndexOf('}');

        if (startIndex >= 0 && endIndex >= 0 && endIndex > startIndex)
        {
            response = response.Substring(startIndex, endIndex - startIndex + 1);
        }

        return response;
    }
}
