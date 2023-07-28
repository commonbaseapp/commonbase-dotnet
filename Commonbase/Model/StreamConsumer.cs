using Newtonsoft.Json;

namespace Commonbase;

internal class StreamConsumer
{
  public static async Task<CompletionResponse?> ReadNextResponseAsync(StreamReader reader)
  {
    string? nextLine = null;
    while (!reader.EndOfStream && string.IsNullOrWhiteSpace(nextLine))
    {
      nextLine = await reader.ReadLineAsync();
    }

    if (string.IsNullOrWhiteSpace(nextLine))
    {
      return null;
    }

    if (!nextLine.StartsWith("data: "))
    {
      throw new StreamingResponseException("Invalid JSON data");
    }

    return JsonConvert.DeserializeObject<CompletionResponse>(nextLine.Substring(6));
  }

  public static async IAsyncEnumerable<CompletionResponse> ConsumeAsync(StreamReader reader)
  {
    var nextResponse = await ReadNextResponseAsync(reader);
    while (nextResponse is not null)
    {
      yield return nextResponse;
      nextResponse = await ReadNextResponseAsync(reader);
    }
  }
}
