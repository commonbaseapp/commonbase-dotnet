using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commonbase.Examples;

public static class FunctionsExample
{
  public static object GetCurrentWeather(string location, string unit = "fahrenheit")
  {
    return new
    {
      location,
      unit,
      temperature = 72,
      forecast = new[] { "sunny", "windy" }
    };
  }

  public static async Task RunAsync()
  {
    CommonbaseClient client = new(apiKey: Program.CB_API_KEY!, projectId: Program.CB_PROJECT_ID);

    List<ChatMessage> messages = new() {
      new ChatMessage { Role = MessageRole.User, Content = "What's the weather like in Boston?" }
    };

    Console.WriteLine("\n=======================================================");
    Console.WriteLine("Functions");
    Console.WriteLine("=======================================================");
    Console.WriteLine("Messages:");
    Console.WriteLine(string.Join("\n", messages.Select(m => $" > {m}")));
    Console.WriteLine("\nResponse:");

    var response = await client.CreateChatCompletionAsync(
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
                { "enum", new [] { "celsius", "farenheit" } }
              }
            },
            required = new [] { "location" }
          }
        },
      },
      providerConfig: new CbOpenAIProviderConfig(CbOpenAIRegion.US, new OpenAIParams
      {
        Model = "gpt-4"
      })
    );

    if (response.BestChoice.FunctionCall is not null)
    {
      var availableFunctions = new Dictionary<string, Delegate> {
        { "get_current_weather", GetCurrentWeather }
      };

      string functionName = response.BestChoice.FunctionCall.Name;

      var functionArguments = JsonConvert.DeserializeObject<JObject>(response.BestChoice.FunctionCall.Arguments!)!;
      var location = functionArguments.Value<string>("location")!;
      var unit = functionArguments.Value<string>("unit")!;

      var functionResponse = GetCurrentWeather(location, unit); //availableFunctions[functionName]

      messages.Add(response.BestChoice.ToAssistantChatMessage());

      messages.Add(new ChatMessage
      {
        Role = MessageRole.Function,
        Name = functionName,
        Content = JsonConvert.SerializeObject(functionResponse)
      });

      var secondResponse = await client.CreateChatCompletionAsync(
        messages
      );

      Console.WriteLine(secondResponse.BestChoice.Text);
    }
  }
}
