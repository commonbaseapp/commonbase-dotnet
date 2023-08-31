namespace Commonbase.Tests;

public class ClientTests
{
  private CommonbaseClient Client;

  public ClientTests()
  {
    Client = new CommonbaseClient(new ClientOptions
    {
      ApiKey = Environment.GetEnvironmentVariable("CB_API_KEY")!,
      ProjectId = Environment.GetEnvironmentVariable("CB_PROJECT_ID")!
    });
  }

  [Fact]
  public void ShouldThrowArgumentExceptionOnNoApiKey()
  {
    Assert.Throws<ArgumentException>(
      () => new CommonbaseClient(apiKey: "")
    );
  }

  [Fact]
  public async void ShouldFailOnInvalidProjectId()
  {
    await Assert.ThrowsAsync<CommonbaseException>(
      () => Client.CreateCompletionAsync(prompt: "", projectId: "")
    );
  }

  [Fact]
  public async void TextCompletion()
  {
    var response = await Client.CreateCompletionAsync(
        prompt: "Hello!"
    );

    Assert.True(response.Choices.Count > 0);
  }

  [Fact]
  public async void ChatCompletion()
  {
    var messages = new[] {
      new ChatMessage { Role = MessageRole.System, Content = "You help people with geography" },
      new ChatMessage { Role = MessageRole.User, Content = "Where is Berlin located?" },
      new ChatMessage { Role = MessageRole.Assistant, Content = "In the EU." },
      new ChatMessage { Role = MessageRole.User, Content = "What country?" },
    };

    var result = await Client.CreateChatCompletionAsync(
      messages: messages,
      providerConfig: new CbOpenAIProviderConfig
      {
        Region = CbOpenAIRegion.EU,
        Params = new OpenAIParams
        {
          Type = RequestType.Chat
        }
      });

    Assert.Contains("germany", result.BestResult.ToLower());
  }

  [Fact]
  public async void StreamCompletion()
  {
    var responses = new List<CompletionResponse?>();

    await foreach (var result in Client.StreamCompletionAsync(
      prompt: "Write me a short essay about artificial intelligence."
    ))
    {
      responses.Add(result);
    }

    Assert.True(responses.Count > 1);
  }

  [Fact]
  public async void NoProviderApiKey()
  {
    await Assert.ThrowsAsync<CommonbaseException>(() => Client.CreateCompletionAsync(
      prompt: "hello",
      providerConfig: new OpenAIProviderConfig(new OpenAIParams
      {
        Type = RequestType.Chat
      })
    ));
  }

  [Fact]
  public async void ValidProviderApiKey()
  {
    var response = await Client.CreateCompletionAsync(
      prompt: "Hello!",
      providerApiKey: Environment.GetEnvironmentVariable("CB_OPENAI_API_KEY"),
      providerConfig: new OpenAIProviderConfig(new OpenAIParams
      {
        Type = RequestType.Chat
      })
    );

    Assert.True(response.Choices.Count > 0);
  }
}
