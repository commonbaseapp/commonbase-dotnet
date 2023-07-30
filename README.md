# Commonbase .NET SDK

Commonbase allows developers to integrate with any popular LLM API provider
without needing to change any code. The SDK helps with collecting data and
feedback from the users and helps you fine-tune models for your specific use case.

## Installation

Install the Commonbase NuGet package using a package manager or the `dotnet` CLI:

```bash
dotnet package add Commonbase
```

## Usage

A project ID is required for all Commonbase requests. You can find your project ID
in the [Commonbase Dashboard](https://commonbase.com/).

To create text and chat completions, use `CommonbaseClient.CreateCompletionAsync`:

```c#
using Commonbase;

CommonbaseClient client = new();

var response = await client.CreateCompletionAsync(
  prompt: "Hello!",
  projectId: "<your_project_id>"
);

Console.WriteLine(response.BestResult);
```

To stream a completion as it is generated, use `CommonbaseClient.StreamCompletionAsync`.

For more examples, see [/Commonbase.Examples](https://github.com/commonbaseapp/commonbase-dotnet/tree/main/Commonbase.Examples).
