
using Newtonsoft.Json;

namespace Commonbase;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record OpenAIProviderConfig : ProviderConfig
{
  public OpenAIProviderConfig(OpenAIParams? parameters = null)
  {
    Params = parameters ?? new OpenAIParams();
  }

  public override string ProviderName => "openai";

  [JsonProperty("params")]
  public OpenAIParams Params { get; }
}

public enum CbOpenAIRegion
{
  Multi,
  EU,
  US
}

public record CbOpenAIProviderConfig : OpenAIProviderConfig
{
  [JsonIgnore]
  public CbOpenAIRegion Region { get; }

  public override string ProviderName => Region is CbOpenAIRegion.Multi ? "cb-openai" : $"cb-openai-{Region.ToString().ToLower()}";

  public CbOpenAIProviderConfig(CbOpenAIRegion region, OpenAIParams? parameters = null) : base(parameters)
  {
    Region = region;
  }
}


[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record OpenAIParams
{
  [JsonProperty("type")]
  internal RequestType Type { get; set; }

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
