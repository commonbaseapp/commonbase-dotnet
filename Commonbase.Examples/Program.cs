namespace Commonbase.Examples;

public class Program
{
  // Set a CB_PROJECT_ID environment variable to run these examples without writing any code.
  // You can also hard-code your project ID here if you prefer.
  public static string? CB_PROJECT_ID = Environment.GetEnvironmentVariable("CB_PROJECT_ID");

  public static async Task Main()
  {
    if (string.IsNullOrWhiteSpace(CB_PROJECT_ID))
    {
      Console.Error.WriteLine("Please set the CB_PROJECT_ID environment variable to your Commonbase Project ID.");
      return;
    }

    await TextCompletionExample.RunAsync();
    await ChatCompletionExample.RunAsync();
    await StreamingCompletionExample.RunAsync();
  }
}
