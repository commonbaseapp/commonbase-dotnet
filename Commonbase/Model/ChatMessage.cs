using Newtonsoft.Json;

namespace Commonbase;

public record ChatMessage
{
  [JsonProperty("role")]
  public required MessageRole Role { get; init; }

  [JsonProperty("content")]
  public required string Content { get; init; }

}
