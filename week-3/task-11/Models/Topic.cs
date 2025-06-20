using Newtonsoft.Json;

namespace AudioTranscriber.Models;

public class Topic
{
    [JsonProperty("topic")]
    public string TopicName { get; set; } = "";
    
    [JsonProperty("mentions")]
    public int Mentions { get; set; }
}
