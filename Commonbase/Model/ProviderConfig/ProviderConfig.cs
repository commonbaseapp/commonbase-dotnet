using Newtonsoft.Json;

namespace Commonbase;

public abstract record ProviderConfig
{
  [JsonProperty("provider")]
  public abstract string ProviderName { get; }
}
