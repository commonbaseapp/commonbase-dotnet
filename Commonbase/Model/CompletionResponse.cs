using Newtonsoft.Json;

namespace Commonbase;

public record CompletionResponse
{
  [JsonProperty("completed")]
  public required bool Completed { get; init; }

  [JsonProperty("invocationId")]
  public required string InvocationId { get; init; }

  [JsonProperty("projectId")]
  public required string ProjectId { get; init; }

  [JsonProperty("type")]
  public required RequestType Type { get; init; }

  [JsonProperty("model")]
  public required string Model { get; init; }

  [JsonProperty("choices")]
  public required IReadOnlyList<CompletionResponseChoice> Choices { get; init; }

  [JsonIgnore]
  public CompletionResponseChoice BestChoice => Choices.FirstOrDefault() ?? new CompletionResponseChoice { Text = "" };
}
