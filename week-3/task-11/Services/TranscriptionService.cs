using OpenAI.Audio;

namespace AudioTranscriber.Services;

public class TranscriptionService(ConfigurationService configurationService)
{
    public async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        string apiKey = configurationService.GetOpenAIApiKey();
        AudioClient client = new("whisper-1", apiKey);

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Text,
        };

        AudioTranscription transcription = await client.TranscribeAudioAsync(audioFilePath, options);
        return transcription.Text;
    }
}
