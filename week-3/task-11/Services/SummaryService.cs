using OpenAI.Chat;

namespace AudioTranscriber.Services;

public class SummaryService(ConfigurationService configurationService)
{
    public async Task<string> GenerateSummaryAsync(string transcription)
    {        string apiKey = configurationService.GetOpenAIApiKey();
        ChatClient client = new("gpt-4.1-mini", apiKey);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                "You are an expert at creating concise, well-structured summaries. " +
                "Create a clear summary that captures the main points and key takeaways from the transcript. " +
                "Focus on the core content and important details while keeping it readable and organized."),
            new UserChatMessage($"Please summarize the following transcript:\n\n{transcription}")
        };

        var chatCompletion = await client.CompleteChatAsync(messages);
        return chatCompletion.Value.Content[0].Text;
    }
}
