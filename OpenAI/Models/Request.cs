using System.Text.Json.Serialization;

namespace OpenAI.Models;

public class Request
{
    [JsonPropertyName("model")]
    public string ModelId { get; set; } = "";
    
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new();
}