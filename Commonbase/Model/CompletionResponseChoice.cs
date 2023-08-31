using Newtonsoft.Json;

namespace Commonbase;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public record CompletionResponseChoice
{
  [JsonProperty("index")]
  public int Index { get; init; }

  [JsonProperty("finish_reason")]
  public string? FinishReason { get; init; }

  [JsonProperty("text")]
  public required string Text { get; init; }

  [JsonProperty("role")]
  public MessageRole? Role { get; init; }

  [JsonProperty("logprobs")]
  public int? Logprobs { get; init; }

  [JsonProperty("function_call")]
  public FunctionCall? FunctionCall { get; init; }

  public ChatMessage ToAssistantChatMessage()
  {
    return new ChatMessage
    {
      Role = MessageRole.Assistant,
      Content = Text,
      FunctionCall = FunctionCall
    };
  }
}
