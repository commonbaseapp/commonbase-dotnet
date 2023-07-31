using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commonbase;

public record ClientOptions(string? ProjectId = null, string? ApiKey = null);

public class CommonbaseClient
{
  private static readonly string ClientVersion;

  static CommonbaseClient()
  {
    var version = typeof(CommonbaseClient).Assembly.GetName().Version;
    ClientVersion = version is null
      ? "0.0.0"
      : $"{version.Major}.{version.Minor}.{version.Build}";
  }

  private HttpClient HttpClient;

  private ClientOptions clientOptions;
  public CommonbaseClient(ClientOptions? options = null)
  {
    clientOptions = options ?? new ClientOptions();
    HttpClient = new HttpClient();
  }

  private async Task<HttpResponseMessage> SendCompletionRequestAsync(
    string prompt,
    string? projectId = null,
    ChatContext? chatContext = null,
    string? userId = null,
    ProviderConfig? providerConfig = null,
    bool stream = false)
  {
    using StringContent body = new(
      JsonConvert.SerializeObject(new
      {
        projectId = projectId ?? clientOptions.ProjectId,
        prompt = prompt,
        apiKey = clientOptions.ApiKey,
        context = chatContext,
        userId = userId,
        providerConfig = providerConfig,
        stream = stream,
      },
        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
      ),
      Encoding.UTF8,
      "application/json"
    );

    using HttpRequestMessage request = new(
      HttpMethod.Post,
      "https://api.commonbase.com/completions"
    );

    request.Content = body;

    request.Headers.Add("User-Agent", $"commonbase-dotnet/{ClientVersion}");

    if (stream)
    {
      request.Headers.Add("Accept", "text/event-stream");
      return await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
    }

    return await HttpClient.SendAsync(request);
  }

  public async Task<CompletionResponse> CreateCompletionAsync(
    string prompt,
    string? projectId = null,
    ChatContext? chatContext = null,
    string? userId = null,
    ProviderConfig? providerConfig = null)
  {
    HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: prompt,
      projectId: projectId,
      chatContext: chatContext,
      userId: userId,
      providerConfig: providerConfig,
      stream: false
    );

    JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

    if (!response.IsSuccessStatusCode)
    {
      throw new CommonbaseException(
        response,
        json.Value<string>("error"),
        json.Value<string>("invocationId")
      );
    }

    return json.ToObject<CompletionResponse>()!;
  }

  public async IAsyncEnumerable<CompletionResponse> StreamCompletionAsync(
    string prompt,
    string? projectId = null,
    ChatContext? chatContext = null,
    string? userId = null,
    ProviderConfig? providerConfig = null)
  {
    using HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: prompt,
      projectId: projectId,
      chatContext: chatContext,
      userId: userId,
      providerConfig: providerConfig,
      stream: true
    );

    if (!response.IsSuccessStatusCode)
    {
      JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
      throw new CommonbaseException(
        response,
        json.Value<string>("error"),
        json.Value<string>("invocationId")
      );
    }

    using var stream = await response.Content.ReadAsStreamAsync();
    using var reader = new StreamReader(stream);
    var consumer = StreamConsumer.ConsumeAsync(reader);

    await foreach (var result in consumer)
    {
      yield return result;
    }
  }
}
