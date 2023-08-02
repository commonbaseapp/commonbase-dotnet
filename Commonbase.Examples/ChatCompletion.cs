namespace Commonbase.Examples;

public static class ChatCompletionExample
{
  public static async Task RunAsync()
  {
    CommonbaseClient client = new(apiKey: Program.CB_API_KEY!);

    string systemMessage = "You help people with geography.";
    ChatMessage[] messages = new[] {
      new ChatMessage { Role = MessageRole.User, Content = "Where is Berlin located?" },
      new ChatMessage { Role = MessageRole.Assistant, Content = "In the EU." },
      new ChatMessage { Role = MessageRole.User, Content = "What country?" },
    };

    Console.WriteLine("\n=======================================================");
    Console.WriteLine("Chat Completion");
    Console.WriteLine("=======================================================");
    Console.WriteLine("Messages:");
    Console.WriteLine($" > System: {systemMessage}");
    Console.WriteLine(string.Join("\n", messages.Select(m => $" > {m}")));
    Console.WriteLine("\nResponse:");

    var response = await client.CreateCompletionAsync(
      prompt: systemMessage,
      chatContext: new ChatContext() { Messages = messages },
      projectId: Program.CB_PROJECT_ID,
      providerConfig: new CbOpenAIProviderConfig
      {
        Region = CbOpenAIRegion.EU,
        Params = new()
        {
          Type = RequestType.Chat
        }
      }
    );

    Console.WriteLine(response.BestResult);
  }
}
