using Newtonsoft.Json;

namespace Commonbase;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record AnthropicParams
{
  [JsonProperty("type")]
  internal RequestType Type => RequestType.Chat;

  [JsonProperty("model")]
  public string? Model { get; init; }

  [JsonProperty("max_tokens_to_sample")]
  public int? MaxTokensToSample { get; init; }

  [JsonProperty("temperature")]
  public float? Temperature { get; init; }

  [JsonProperty("stop_sequences")]
  public string[]? StopSequences { get; init; }

  [JsonProperty("top_k")]
  public float? TopK { get; init; }

  [JsonProperty("top_p")]
  public float? TopP { get; init; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record AnthropicProviderConfig : ProviderConfig
{
  public AnthropicProviderConfig(AnthropicParams? parameters = null)
  {
    Params = parameters;
  }

  public override string ProviderName => "anthropic";

  [JsonProperty("params")]
  public AnthropicParams? Params { get; }
}
