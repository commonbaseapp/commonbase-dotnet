namespace Commonbase.Examples;

public static class ChatCompletionExample
{
  public static async Task RunAsync()
  {
    CommonbaseClient client = new(apiKey: Program.CB_API_KEY!, projectId: Program.CB_PROJECT_ID);

    ChatMessage[] messages = new[] {
      new ChatMessage { Role = MessageRole.System, Content = "You help people with geography." },
      new ChatMessage { Role = MessageRole.User, Content = "Where is Berlin located?" },
      new ChatMessage { Role = MessageRole.Assistant, Content = "In the EU." },
      new ChatMessage { Role = MessageRole.User, Content = "What country?" },
    };

    Console.WriteLine("\n=======================================================");
    Console.WriteLine("Chat Completion");
    Console.WriteLine("=======================================================");
    Console.WriteLine("Messages:");
    Console.WriteLine(string.Join("\n", messages.Select(m => $" > {m}")));
    Console.WriteLine("\nResponse:");

    var response = await client.CreateChatCompletionAsync(
      messages: messages,
      providerConfig: new CbOpenAIProviderConfig
      {
        Region = CbOpenAIRegion.EU,
        Params = new()
        {
          Type = RequestType.Chat
        }
      }
    );

    Console.WriteLine(response.BestChoice.Text);
  }
}
