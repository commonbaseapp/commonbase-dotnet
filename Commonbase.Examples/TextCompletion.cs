
namespace Commonbase.Examples;

public static class TextCompletionExample
{
  public static async Task RunAsync()
  {
    CommonbaseClient client = new();
    string prompt = "Hello, what is your name?";

    var response = await client.CreateCompletionAsync(
      prompt,
      projectId: Program.CB_PROJECT_ID
    );

    Console.WriteLine($@"
Text Completion
=======================================================
Prompt:
-------------------------------------------------------
{prompt}
-------------------------------------------------------

Response:
-------------------------------------------------------
{response.BestResult}
-------------------------------------------------------
=======================================================
");
  }
}
