namespace Commonbase.Examples;

public static class TextCompletionExample
{
  public static async Task RunAsync()
  {
    CommonbaseClient client = new(apiKey: Program.CB_API_KEY!, projectId: Program.CB_PROJECT_ID);

    string prompt = "Hello, what is your name?";

    Console.WriteLine("\n=======================================================");
    Console.WriteLine("Text Completion");
    Console.WriteLine("=======================================================");
    Console.WriteLine("Prompt:");
    Console.WriteLine($" > {prompt}");
    Console.WriteLine("\nResponse:");

    var response = await client.CreateCompletionAsync(
      prompt
    );

    Console.WriteLine(response.BestChoice.Text);
  }
}
