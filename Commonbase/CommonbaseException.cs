namespace Commonbase;

public class CommonbaseException : Exception
{
  public HttpResponseMessage HttpResponse { get; }
  public string Error { get; }
  public string InvocationId { get; }

  public CommonbaseException(HttpResponseMessage response, string? error, string? invocationId) : base(error)
  {
    HttpResponse = response;
    Error = error ?? string.Empty;
    InvocationId = invocationId ?? string.Empty;
  }
}
