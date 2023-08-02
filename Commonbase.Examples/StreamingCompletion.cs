namespace Commonbase.Examples;

public static class StreamingCompletionExample
{
  public static async Task RunAsync()
  {
    CommonbaseClient client = new(apiKey: Program.CB_API_KEY!);

    string prompt = "Write me an essay about artificial intelligence.";

    var stream = client.StreamCompletionAsync(
      prompt,
      projectId: Program.CB_PROJECT_ID
    );

    Console.WriteLine("\n=======================================================");
    Console.WriteLine("Streaming Completion");
    Console.WriteLine("=======================================================");
    Console.WriteLine("Prompt:");
    Console.WriteLine($" > {prompt}");
    Console.WriteLine("\nResponse:");

    await foreach (var response in stream)
    {
      if (!response.Completed)
        Console.Write(response.BestResult);
      else
        Console.WriteLine();
    }
  }
}
