using Newtonsoft.Json;

namespace Commonbase;

public record FunctionCall
{
  [JsonProperty("name")]
  public required string Name { get; init; }

  [JsonProperty("arguments", NullValueHandling = NullValueHandling.Ignore)]
  public string? Arguments { get; init; }
}
