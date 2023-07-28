﻿using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commonbase;

public record ClientOptions(string? ProjectId = null, string? ApiKey = null);

public class CommonbaseClient
{
  private HttpClient HttpClient;

  private ClientOptions clientOptions;
  public CommonbaseClient(ClientOptions? options)
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

    if (stream)
    {
      request.Headers.Add("Accept", "text/event-stream");
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
}
