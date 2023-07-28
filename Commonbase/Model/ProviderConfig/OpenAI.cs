
using Newtonsoft.Json;

namespace Commonbase;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record OpenAIProviderConfig : ProviderConfig
{
  public OpenAIProviderConfig(OpenAIParams? p)
  {
    Params = p;
  }

  public override string ProviderName => "openai";

  [JsonProperty("params")]
  public OpenAIParams? Params { get; init; }
}

public enum CbOpenAIRegion
{
  EU
}

public record CbOpenAIProviderConfig : ProviderConfig
{
  public required CbOpenAIRegion Region { get; init; }
  public override string ProviderName => $"cb-openai-{Region.ToString().ToLower()}";

  [JsonProperty("params")]
  public OpenAIParams? Params { get; init; }
}


[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record OpenAIParams
{
  [JsonProperty("type")]
  public required RequestType Type { get; init; }

  [JsonProperty("model")]
  public string? Model { get; init; }

  [JsonProperty("temperature")]
  public float? Temperature { get; init; }

  [JsonProperty("top_p")]
  public float? TopP { get; init; }

  [JsonProperty("max_tokens")]
  public int? MaxTokens { get; init; }

  [JsonProperty("n")]
  public int? N { get; init; }

  [JsonProperty("frequency_penalty")]
  public float? FrequencyPenalty { get; init; }

  [JsonProperty("presence_penalty")]
  public float? PresencePenalty { get; init; }

  [JsonProperty("stop")]
  public string[]? Stop { get; init; }

  [JsonProperty("best_of")]
  public int? BestOf { get; init; }

  [JsonProperty("suffix")]
  public string? Suffix { get; init; }

  [JsonProperty("logprobs")]
  public int? Logprobs { get; init; }
}
