using AudioTranscriber.Models;

namespace AudioTranscriber.Services;

public static class DisplayService
{
    public static void DisplayResults(string summary, AudioAnalytics analytics)
    {
        Console.WriteLine("🎯 RESULTS");
        Console.WriteLine("=" + new string('=', 50));
        
        Console.WriteLine("\n📝 SUMMARY:");
        Console.WriteLine(new string('-', 30));
        Console.WriteLine(summary);
        
        Console.WriteLine($"\n📊 ANALYTICS:");
        Console.WriteLine(new string('-', 30));
        Console.WriteLine($"• Word Count: {analytics.WordCount:N0}");
        Console.WriteLine($"• Speaking Speed: {analytics.SpeakingSpeedWpm} WPM");
        Console.WriteLine($"• Top Topics:");
        
        foreach (var topic in analytics.FrequentlyMentionedTopics)
        {
            Console.WriteLine($"  - {topic.TopicName}: {topic.Mentions} mentions");
        }
    }

    public static void DisplayError(Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
    }

    public static void DisplayProcessingStep(string step, string? details = null)
    {
        Console.WriteLine($"{step}");
        if (!string.IsNullOrEmpty(details))
        {
            Console.WriteLine($"✅ {details}\n");
        }
    }
}
