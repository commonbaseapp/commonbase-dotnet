using Newtonsoft.Json;

namespace Commonbase;

public record ChatContext
{
  [JsonProperty("messages")]
  public required IEnumerable<ChatMessage> Messages { get; init; }
}
