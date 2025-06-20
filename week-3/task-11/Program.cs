using AudioTranscriber.Services;
using AudioTranscriber.Models;

namespace AudioTranscriber;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Audio Transcriber with AI Analysis ===\n");

        // Initialize services
        var configurationService = new ConfigurationService();
        var transcriptionService = new TranscriptionService(configurationService);
        var summaryService = new SummaryService(configurationService);
        var analyticsService = new AnalyticsService(configurationService);

        try
        {
            // Get audio file path
            string audioFilePath = FileService.GetAudioFilePath(args);
            
            if (!File.Exists(audioFilePath))
            {
                Console.WriteLine($"Error: Audio file not found at '{audioFilePath}'");
                return;
            }

            Console.WriteLine($"Processing audio file: {Path.GetFileName(audioFilePath)}\n");

            // Step 1: Transcribe audio
            DisplayService.DisplayProcessingStep("🎵 Transcribing audio...");
            string transcription = await transcriptionService.TranscribeAudioAsync(audioFilePath);
            
            // Save transcription to file
            string transcriptionFileName = FileService.SaveTranscription(transcription);
            DisplayService.DisplayProcessingStep("", $"Transcription saved to: {transcriptionFileName}");

            // Step 2: Generate summary
            DisplayService.DisplayProcessingStep("📝 Generating summary...");
            string summary = await summaryService.GenerateSummaryAsync(transcription);
            
            // Save summary to file
            string summaryFileName = FileService.SaveSummary(summary);
            DisplayService.DisplayProcessingStep("", $"Summary saved to: {summaryFileName}");
            
            // Step 3: Generate analytics
            DisplayService.DisplayProcessingStep("📊 Analyzing transcript...");
            AudioAnalytics analytics = await analyticsService.GenerateAnalyticsAsync(transcription, audioFilePath);
            
            // Save analytics to file
            string analyticsFileName = FileService.SaveAnalytics(analytics);
            DisplayService.DisplayProcessingStep("", $"Analytics saved to: {analyticsFileName}");

            // Display results in console
            DisplayService.DisplayResults(summary, analytics);
        }
        catch (Exception ex)
        {
            DisplayService.DisplayError(ex);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
