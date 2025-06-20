using AudioTranscriber.Models;
using Newtonsoft.Json;
using System.Text;

namespace AudioTranscriber.Services;

public static class FileService
{
    public static string SaveTranscription(string transcription)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"transcription_{timestamp}.md";
        
        var content = new StringBuilder();
        content.AppendLine("# Audio Transcription");
        content.AppendLine();
        content.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        content.AppendLine();
        content.AppendLine("## Transcript");
        content.AppendLine();
        content.AppendLine(transcription);
        
        File.WriteAllText(fileName, content.ToString());
        return fileName;
    }

    public static string SaveSummary(string summary)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"summary_{timestamp}.md";
        
        var content = new StringBuilder();
        content.AppendLine("# Audio Summary");
        content.AppendLine();
        content.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        content.AppendLine();
        content.AppendLine("## Summary");
        content.AppendLine();
        content.AppendLine(summary);
        
        File.WriteAllText(fileName, content.ToString());
        return fileName;
    }

    public static string SaveAnalytics(AudioAnalytics analytics)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"analysis_{timestamp}.json";
        
        string json = JsonConvert.SerializeObject(analytics, Formatting.Indented);
        File.WriteAllText(fileName, json);
        return fileName;
    }

    public static string GetAudioFilePath(string[] args)
    {
        if (args.Length > 0)
        {
            return Path.GetFullPath(args[0]);
        }

        // Default to CAR0004.mp3 if no argument provided
        string defaultFile = Path.Combine(Directory.GetCurrentDirectory(), "CAR0004.mp3");
        if (File.Exists(defaultFile))
        {
            return defaultFile;
        }

        Console.Write("Enter the path to your audio file: ");
        string? input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? "" : Path.GetFullPath(input);
    }
}
