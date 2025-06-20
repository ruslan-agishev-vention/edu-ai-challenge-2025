using Newtonsoft.Json;

namespace AudioTranscriber.Models;

public class TopicsResponse
{
    [JsonProperty("topics")]
    public List<Topic> Topics { get; set; } = new();
}
