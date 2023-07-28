namespace Commonbase.Tests;

public class ClientTests
{
  private CommonbaseClient Client;

  public ClientTests()
  {
    Client = new CommonbaseClient(new ClientOptions
    {
      ProjectId = Environment.GetEnvironmentVariable("CB_PROJECT_ID")
    });
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
    var context = new ChatContext
    {
      Messages = new[] {
        new ChatMessage { Role = MessageRole.User, Content = "Where is Berlin located?" },
        new ChatMessage { Role = MessageRole.Assistant, Content = "In the EU." },
        new ChatMessage { Role = MessageRole.User, Content = "What country?" },
      }
    };

    var result = await Client.CreateCompletionAsync(
      prompt: "You help people with geography",
      chatContext: context,
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

    Assert.True(responses.Count > 0);
  }
}
