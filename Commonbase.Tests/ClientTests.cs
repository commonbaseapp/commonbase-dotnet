using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commonbase.Tests;

public class ClientTests
{
  private CommonbaseClient Client;

  public ClientTests()
  {
    Client = new CommonbaseClient(
      apiKey: Environment.GetEnvironmentVariable("CB_API_KEY")!,
      projectId: Environment.GetEnvironmentVariable("CB_PROJECT_ID")!
    );
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

    Assert.Contains("germany", result.BestChoice.Text.ToLower());
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

  [Fact]
  public async void Functions()
  {
    List<ChatMessage> messages = new() {
      new ChatMessage { Role = MessageRole.User, Content = "What's the weather like in Boston?" }
    };

    var response = await Client.CreateChatCompletionAsync(
      messages: messages,
      functions: new[] {
        new ChatFunction {
          Name = "get_current_weather",
          Description = "Get the current weather in a given location",
          Parameters = new {
            type = "object",
            properties = new {
              location = new {
                type = "string",
                description = "The city and state, e.g., San Francisco, CA"
              },
              unit = new Dictionary<string, object> {
                { "type", "string" },
                { "enum", new [] { "celsius", "fahrenheit" } }
              }
            },
            required = new [] { "location" }
          }
        },
      },
      providerConfig: new CbOpenAIProviderConfig
      {
        Region = CbOpenAIRegion.US,
        Params = new()
        {
          Type = RequestType.Chat,
          Model = "gpt-4"
        }
      }
    );

    Assert.NotNull(response.BestChoice.FunctionCall);

    string functionName = response.BestChoice.FunctionCall.Name;

    Assert.Equal("get_current_weather", functionName);

    var functionArguments = JsonConvert.DeserializeObject<JObject>(response.BestChoice.FunctionCall.Arguments!)!;
    var location = functionArguments.Value<string>("location")!;
    var unit = functionArguments.Value<string>("unit")!;

    Assert.Contains("boston", location.ToLower());

    var functionResponse = new
    {
      location,
      unit = unit ?? "fahrenheit",
      temperature = 72,
      forecast = new[] { "sunny", "windy" }
    };

    messages.Add(response.BestChoice.ToAssistantChatMessage());

    messages.Add(new ChatMessage
    {
      Role = MessageRole.Function,
      Name = functionName,
      Content = JsonConvert.SerializeObject(functionResponse)
    });

    var secondResponse = await Client.CreateChatCompletionAsync(
      messages
    );

    Assert.False(string.IsNullOrWhiteSpace(secondResponse.BestChoice.Text));
  }
}
