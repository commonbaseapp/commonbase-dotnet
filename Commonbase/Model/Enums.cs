using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Commonbase;

[JsonConverter(typeof(StringEnumConverter))]
public enum RequestType
{
  [EnumMember(Value = "text")]
  Text,

  [EnumMember(Value = "chat")]
  Chat,

  [EnumMember(Value = "embeddings")]
  Embeddings
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageRole
{
  [EnumMember(Value = "system")]
  System,

  [EnumMember(Value = "user")]
  User,

  [EnumMember(Value = "assistant")]
  Assistant
}
