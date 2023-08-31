namespace Commonbase.Examples;

public class Program
{
  // Set a CB_PROJECT_ID environment variable to run these examples without writing any code.
  // You can also hard-code your project ID here if you prefer.
  public static string? CB_PROJECT_ID = Environment.GetEnvironmentVariable("CB_PROJECT_ID");
  public static string? CB_API_KEY = Environment.GetEnvironmentVariable("CB_API_KEY");

  public static async Task Main()
  {
    if (string.IsNullOrWhiteSpace(CB_PROJECT_ID) || string.IsNullOrWhiteSpace(CB_API_KEY))
    {
      Console.Error.WriteLine(
        "Please set the CB_PROJECT_ID and CB_API_KEY environment variables to run these examples."
      );
      return;
    }

    await TextCompletionExample.RunAsync();
    await ChatCompletionExample.RunAsync();
    await StreamingCompletionExample.RunAsync();
    await FunctionsExample.RunAsync();
  }
}
