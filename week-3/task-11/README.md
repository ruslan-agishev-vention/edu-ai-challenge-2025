# Audio Transcriber with AI Analysis

A C# .NET 8 console application that transcribes audio files using OpenAI's Whisper API, generates summaries using GPT, and provides detailed analytics including word count, speaking speed, and frequently mentioned topics.

## Features

- 🎵 **Audio Transcription**: Uses OpenAI's Whisper-1 model for high-accuracy speech-to-text conversion
- 📝 **AI-Powered Summarization**: Generates concise summaries using GPT-4
- 📊 **Advanced Analytics**: Extracts metrics including:
  - Total word count
  - Speaking speed (words per minute)
  - Top frequently mentioned topics with mention counts
- 💾 **File Output**: Saves results to separate files with timestamps
- 🔧 **Flexible Input**: Accepts any audio file format supported by Whisper API

## Supported Audio Formats

The application supports all audio formats compatible with OpenAI's Whisper API:
- MP3, MP4, MPEG, MPGA, M4A, WAV, WEBM
- Maximum file size: 25 MB

## Prerequisites

- .NET 8.0 SDK or later
- OpenAI API account and API key
- Audio file to transcribe

## Installation & Setup

### 1. Clone or Download the Project

```bash
# If using git
git clone <repository-url>
cd week-3/task-11

# Or download and extract the project files
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Configure OpenAI API Key

You have two options to set your OpenAI API key:

#### Option A: Configuration File (Recommended)

Create `appsettings.json` and replace `YOUR_OPENAI_API_KEY_HERE` with your actual API key:
```json
{
  "OpenAI": {
    "ApiKey": "sk-your-actual-openai-api-key-here"
  }
}
```

#### Option B: Environment Variable

**Windows (Command Prompt):**
```cmd
set OPENAI_API_KEY=your-openai-api-key-here
```

**Windows (PowerShell):**
```powershell
$env:OPENAI_API_KEY="your-openai-api-key-here"
```

**Linux/macOS:**
```bash
export OPENAI_API_KEY="your-openai-api-key-here"
```

### 4. Build the Application

```bash
dotnet build
```

## Usage

### Basic Usage (Default Audio File)

If you have `CAR0004.mp3` in the project directory:

```bash
dotnet run
```

### Specify Audio File

```bash
dotnet run "path/to/your/audio-file.mp3"
```

### Examples

```bash
# Using relative path
dotnet run "audio/interview.wav"

# Using absolute path
dotnet run "C:\Users\YourName\Documents\recording.mp3"
```

## Output Files

The application generates timestamped files for each run:

1. **`transcription_YYYYMMDD_HHMMSS.md`** - Full transcription in Markdown format
2. **`summary_YYYYMMDD_HHMMSS.md`** - AI-generated summary in Markdown format  
3. **`analysis_YYYYMMDD_HHMMSS.json`** - Analytics data in JSON format

### Sample Analytics Output

```json
{
  "word_count": 1280,
  "speaking_speed_wpm": 132,
  "frequently_mentioned_topics": [
    { "topic": "Customer Onboarding", "mentions": 6 },
    { "topic": "Q4 Roadmap", "mentions": 4 },
    { "topic": "AI Integration", "mentions": 3 }
  ]
}
```

## Console Output

The application provides real-time feedback and displays:

- Processing status for each step
- File paths where results are saved
- Complete summary in the console
- Analytics overview with key metrics

## Error Handling

The application handles common scenarios:

- **Missing API Key**: Clear instructions on how to configure it
- **File Not Found**: Prompts for correct file path
- **Invalid Audio Format**: OpenAI API will return appropriate error
- **Network Issues**: Displays connection error details
- **API Limits**: Shows quota or rate limit information

## Project Structure

```
AudioTranscriber/
├── AudioTranscriber.csproj    # Project file with dependencies
├── Program.cs                 # Main application code
├── README.md                  # This file
├── CAR0004.mp3               # Sample audio file
└── Output files generated:
    ├── transcription_*.md
    ├── summary_*.md
    └── analysis_*.json
```

## Dependencies

- **OpenAI** (v1.11.0) - OpenAI API client
- **Newtonsoft.Json** (v13.0.3) - JSON serialization
- **Microsoft.Extensions.Configuration** (v8.0.0) - Configuration management
- **Microsoft.Extensions.Configuration.Json** (v8.0.0) - JSON configuration
- **Microsoft.Extensions.Configuration.UserSecrets** (v8.0.0) - User secrets support

## API Usage

This application uses:
- **Whisper-1 Model**: For audio transcription
- **GPT-4 Model**: For text summarization and topic analysis

## Cost Considerations

- Whisper API: $0.006 per minute of audio
- GPT-4 API: Varies based on token usage for summary and analysis

## Troubleshooting

### Common Issues

**"OpenAI API key not found"**
- Ensure you've set the API key using one of the methods in step 3
- Verify the key is correct and has sufficient credits

**"Audio file not found"**
- Check the file path is correct
- Ensure the file exists and is accessible
- Try using absolute path instead of relative path

**"File format not supported"**
- Ensure your audio file is in a supported format (MP3, WAV, etc.)
- Check file size is under 25 MB limit

**Network connectivity issues**
- Verify internet connection
- Check if firewall is blocking the application
- Ensure OpenAI API is accessible from your network

## Security Notes

- Never commit your API key to version control
- Use User Secrets for development
- Use secure environment variables in production
- The generated files contain transcribed content - handle according to your privacy requirements

## License

This project is provided as-is for educational and development purposes.

## Support

For issues related to:
- OpenAI API: Check [OpenAI Documentation](https://platform.openai.com/docs)
- .NET: Check [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet/)
- This application: Review the troubleshooting section above
