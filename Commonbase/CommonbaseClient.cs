﻿using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commonbase;

public record ClientOptions
{
  public required string ApiKey { get; init; }
  public string? ProjectId { get; init; }
}

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
  public CommonbaseClient(string apiKey, string? projectId = null)
    : this(new ClientOptions { ApiKey = apiKey, ProjectId = projectId }) { }
  public CommonbaseClient(ClientOptions options)
  {
    if (string.IsNullOrWhiteSpace(options.ApiKey))
    {
      throw new ArgumentException("Api Key must not be null or empty.", nameof(options.ApiKey));
    }

    clientOptions = options;
    HttpClient = new HttpClient();
  }

  private async Task<HttpResponseMessage> SendCompletionRequestAsync(
    string? prompt,
    IEnumerable<ChatMessage>? messages,
    IEnumerable<ChatFunction>? functions,
    string? functionCall,
    string? projectId,
    string? userId,
    string? providerApiKey,
    ProviderConfig? providerConfig,
    bool stream,
    RequestType type)
  {
    providerConfig ??= new CbOpenAIProviderConfig(CbOpenAIRegion.EU);
    if (providerConfig is OpenAIProviderConfig openAiConfig)
    {
      openAiConfig.Params.Type = type;
    }

    using StringContent body = new(
      JsonConvert.SerializeObject(new
      {
        projectId = projectId ?? clientOptions.ProjectId,
        prompt = prompt,
        apiKey = clientOptions.ApiKey,
        messages = messages,
        userId = userId,
        providerConfig = providerConfig,
        stream = stream,
        functions = functions,
        functionCall = functionCall is null or "none" or "auto"
          ? functionCall as object
          : new { name = functionCall }
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

    request.Headers.Add("Authorization", clientOptions.ApiKey);
    request.Headers.Add("User-Agent", $"commonbase-dotnet/{ClientVersion}");

    if (!string.IsNullOrWhiteSpace(providerApiKey))
    {
      request.Headers.Add("Provider-API-Key", providerApiKey);
    }

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
    string? userId = null,
    string? providerApiKey = null,
    OpenAIProviderConfig? providerConfig = null)
  {
    HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: prompt,
      messages: null,
      functions: null,
      functionCall: null,
      projectId: projectId,
      userId: userId,
      providerApiKey: providerApiKey,
      providerConfig: providerConfig,
      stream: false,
      type: RequestType.Text
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
    string? userId = null,
    string? providerApiKey = null,
    OpenAIProviderConfig? providerConfig = null)
  {
    using HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: prompt,
      messages: null,
      functions: null,
      functionCall: null,
      projectId: projectId,
      userId: userId,
      providerApiKey: providerApiKey,
      providerConfig: providerConfig,
      stream: true,
      type: RequestType.Text
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


  public async Task<CompletionResponse> CreateChatCompletionAsync(
    IEnumerable<ChatMessage> messages,
    IEnumerable<ChatFunction>? functions = null,
    string? functionCall = null,
    string? projectId = null,
    string? userId = null,
    string? providerApiKey = null,
    ProviderConfig? providerConfig = null)
  {
    HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: null,
      messages: messages,
      functions: functions,
      functionCall: functionCall,
      projectId: projectId,
      userId: userId,
      providerApiKey: providerApiKey,
      providerConfig: providerConfig,
      stream: false,
      type: RequestType.Chat
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

  public async IAsyncEnumerable<CompletionResponse> StreamChatCompletionAsync(
    IEnumerable<ChatMessage> messages,
    IEnumerable<ChatFunction>? functions = null,
    string? functionCall = null,
    string? projectId = null,
    string? userId = null,
    string? providerApiKey = null,
    ProviderConfig? providerConfig = null)
  {
    using HttpResponseMessage response = await SendCompletionRequestAsync(
      prompt: null,
      messages: messages,
      functions: functions,
      functionCall: functionCall,
      projectId: projectId,
      userId: userId,
      providerApiKey: providerApiKey,
      providerConfig: providerConfig,
      stream: true,
      type: RequestType.Chat
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
