using AudioTranscriber.Models;
using OpenAI.Chat;
using Newtonsoft.Json;
using NAudio.Wave;

namespace AudioTranscriber.Services;

public class AnalyticsService(ConfigurationService configurationService)
{
    public async Task<AudioAnalytics> GenerateAnalyticsAsync(string transcription, string audioFilePath)
    {
        // Calculate basic metrics
        string[] words = transcription.Split([' ', '\n', '\r', '\t'], 
            StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        // Calculate speaking speed using actual audio duration
        double audioDurationMinutes = GetAudioDurationInMinutes(audioFilePath);
        int speakingSpeedWpm = audioDurationMinutes > 0 
            ? (int)Math.Round(wordCount / audioDurationMinutes) 
            : 0;

        // Use GPT to identify frequently mentioned topics
        var topics = await AnalyzeTopicsAsync(transcription);

        return new AudioAnalytics
        {
            WordCount = wordCount,
            SpeakingSpeedWpm = speakingSpeedWpm,
            FrequentlyMentionedTopics = topics.Take(5).ToList()
        };
    }

    private async Task<List<Topic>> AnalyzeTopicsAsync(string transcription)
    {
        try
        {
            string apiKey = configurationService.GetOpenAIApiKey();
            ChatClient client = new("gpt-4.1-mini", apiKey);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are an expert at analyzing text and identifying key topics. " +
                    "Analyze the transcript and identify the most frequently mentioned topics. " +
                    "Return your response as a JSON object with this exact structure: " +
                    "{ \"topics\": [{ \"topic\": \"Topic Name\", \"mentions\": number }] }. " +
                    "Focus on substantial topics, not common words. Identify at least 3-5 main topics."),
                new UserChatMessage($"Analyze this transcript and identify frequently mentioned topics:\n\n{transcription}")
            };

            var chatCompletion = await client.CompleteChatAsync(messages);
            string topicsResponse = chatCompletion.Value.Content[0].Text;

            var topicsData = JsonConvert.DeserializeObject<TopicsResponse>(topicsResponse);
            return topicsData?.Topics ?? new List<Topic>();
        }
        catch
        {
            // Fallback: create basic topics from word frequency
            return GetBasicTopicsFromText(transcription);
        }
    }

    private static List<Topic> GetBasicTopicsFromText(string text)
    {
        // Simple fallback method to extract common meaningful words
        var words = text.ToLower()
            .Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, 
                StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 4 && !IsCommonWord(w))
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new Topic 
            { 
                TopicName = CapitalizeFirst(g.Key), 
                Mentions = g.Count() 
            })
            .ToList();

        return words;
    }

    private static bool IsCommonWord(string word)
    {
        var commonWords = new HashSet<string> 
        { 
            "that", "this", "with", "have", "will", "been", "from", "they", 
            "were", "said", "each", "which", "their", "would", "there", 
            "could", "other", "than", "very", "what", "know", "just", 
            "first", "also", "after", "back", "through", "where", "much", "should" 
        };

        return commonWords.Contains(word);
    }

    private static string CapitalizeFirst(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        return char.ToUpper(input[0]) + input.Substring(1);
    }
    
    private static double GetAudioDurationInMinutes(string audioFilePath)
    {
        try
        {
            string extension = Path.GetExtension(audioFilePath).ToLower();
            
            TimeSpan duration;
            
            switch (extension)
            {
                case ".mp3":
                    using (var mp3Reader = new Mp3FileReader(audioFilePath))
                    {
                        duration = mp3Reader.TotalTime;
                    }
                    break;
                    
                case ".wav":
                    using (var waveReader = new WaveFileReader(audioFilePath))
                    {
                        duration = waveReader.TotalTime;
                    }
                    break;
                    
                case ".m4a":
                case ".mp4":
                    // For M4A/MP4, we can use MediaFoundationReader if available
                    try
                    {
                        using var mediaReader = new MediaFoundationReader(audioFilePath);
                        duration = mediaReader.TotalTime;
                    }
                    catch
                    {
                        // Fallback to file size estimation for M4A if MediaFoundation fails
                        return GetEstimatedDurationFromFileSize(audioFilePath);
                    }
                    break;
                    
                default:
                    // For other formats, try MediaFoundationReader first, then fallback
                    try
                    {
                        using var mediaReader = new MediaFoundationReader(audioFilePath);
                        duration = mediaReader.TotalTime;
                    }
                    catch
                    {
                        return GetEstimatedDurationFromFileSize(audioFilePath);
                    }
                    break;
            }
            
            double durationMinutes = duration.TotalMinutes;
            
            // Sanity check
            if (durationMinutes <= 0 || durationMinutes > 120) // More than 2 hours seems unlikely
            {
                return GetEstimatedDurationFromFileSize(audioFilePath);
            }
            
            return durationMinutes;
        }
        catch
        {
            // Fallback to file size estimation
            return GetEstimatedDurationFromFileSize(audioFilePath);
        }
    }
    
    private static double GetEstimatedDurationFromFileSize(string audioFilePath)
    {
        try
        {
            var fileInfo = new FileInfo(audioFilePath);
            long fileSizeBytes = fileInfo.Length;
            string extension = Path.GetExtension(audioFilePath).ToLower();
            
            // Estimate duration based on file size and format
            double estimatedMinutes = extension switch
            {
                ".mp3" => fileSizeBytes / (128.0 * 1024 / 8 * 60), // 128 kbps MP3
                ".wav" => fileSizeBytes / (1411.0 * 1024 / 8 * 60), // 44.1kHz 16-bit stereo WAV
                ".m4a" => fileSizeBytes / (128.0 * 1024 / 8 * 60), // 128 kbps AAC
                _ => fileSizeBytes / (128.0 * 1024 / 8 * 60) // Default estimation
            };
            
            return Math.Max(0.1, estimatedMinutes); // Minimum 0.1 minutes
        }
        catch
        {
            return 1.0; // Default to 1 minute if all else fails
        }
    }
}
