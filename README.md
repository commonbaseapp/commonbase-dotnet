# Commonbase .NET SDK

[![NuGet version](https://badge.fury.io/nu/Commonbase.svg)](https://badge.fury.io/nu/Commonbase)

Commonbase allows developers to integrate with any popular LLM API provider
without needing to change any code. The SDK helps with collecting data and
feedback from the users and helps you fine-tune models for your specific use case.

## Installation

Install the Commonbase NuGet package using a package manager or the `dotnet` CLI:

```bash
dotnet package add Commonbase
```

## Usage

A Project ID and API Key are required for all Commonbase requests. You can find your Project ID
and generate an API Key in the [Commonbase Dashboard](https://commonbase.com/).

To create a completion, configure a `CommonbaseClient` with your API Key and provide your Project
ID and prompt to `CreateCompletionAsync`:

```c#
using Commonbase;

CommonbaseClient client = new(apiKey: "API_KEY");

var response = await client.CreateCompletionAsync(
  prompt: "Hello!",
  projectId: "PROJECT_ID"
);

Console.WriteLine(response.BestResult);
```

To stream a completion as it is generated, use `StreamCompletionAsync`.

For more examples, see [/Commonbase.Examples](https://github.com/commonbaseapp/commonbase-dotnet/tree/main/Commonbase.Examples).
