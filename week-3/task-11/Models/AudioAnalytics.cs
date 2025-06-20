using Newtonsoft.Json;

namespace AudioTranscriber.Models;

public class AudioAnalytics
{
    [JsonProperty("word_count")]
    public int WordCount { get; set; }
    
    [JsonProperty("speaking_speed_wpm")]
    public int SpeakingSpeedWpm { get; set; }
    
    [JsonProperty("frequently_mentioned_topics")]
    public List<Topic> FrequentlyMentionedTopics { get; set; } = new();
}
