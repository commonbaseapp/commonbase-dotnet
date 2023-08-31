using Newtonsoft.Json;

namespace Commonbase;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record ChatMessage
{
  [JsonProperty("role")]
  public required MessageRole Role { get; init; }

  [JsonProperty("content")]
  public required string Content { get; init; }

  [JsonProperty("function_call")]
  public FunctionCall? FunctionCall { get; init; }

  [JsonProperty("name")]
  public string? Name { get; init; }

}

public record ChatFunction
{
  [JsonProperty("name")]
  public required string Name { get; init; }

  [JsonProperty("description")]
  public required string Description { get; init; }

  [JsonProperty("parameters")]
  public required object Parameters { get; init; }
}
