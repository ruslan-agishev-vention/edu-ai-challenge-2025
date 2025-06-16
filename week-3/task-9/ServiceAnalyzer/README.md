# Service Analyzer

A .NET console application that uses OpenAI's GPT-4 model to analyze digital services and generate comprehensive markdown reports from multiple perspectives.

## Features

- **Dual Input Support**: Accept either known service names (e.g., "Spotify", "Notion") or raw service description text
- **AI-Powered Analysis**: Uses OpenAI GPT-4.1 to generate intelligent insights and structured reports
- **Comprehensive Reports**: Generates 8-section markdown reports covering business, technical, and user perspectives
- **File Export**: Option to save generated reports as markdown files
- **Secure API Key Management**: Multiple ways to configure your OpenAI API key securely

## Report Sections

Each generated report includes the following sections:

1. **Brief History** - Founding year, key milestones, and company background
2. **Target Audience** - Primary user segments and demographics
3. **Core Features** - Top 2-4 key functionalities and capabilities
4. **Unique Selling Points** - Key differentiators from competitors
5. **Business Model** - Revenue streams and monetization strategies
6. **Tech Stack Insights** - Technologies, platforms, and infrastructure hints
7. **Perceived Strengths** - Highlighted positives and standout features
8. **Perceived Weaknesses** - Known limitations and cited drawbacks

## Prerequisites

- .NET 8.0 SDK or later
- OpenAI API key (from [OpenAI Platform](https://platform.openai.com/api-keys))

## Installation

1. **Clone or download the project:**
   ```bash
   git clone <repository-url>
   cd ServiceAnalyzer
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the application:**
   ```bash
   dotnet build
   ```

## Configuration

### Setting up your OpenAI API Key

⚠️ **Important**: Never commit your API key to version control!

Choose one of the following methods to configure your OpenAI API key:

#### Method 1: Environment Variable (Recommended)
```bash
# Windows (PowerShell)
$env:OPENAI_API_KEY="your_openai_api_key_here"

# Windows (Command Prompt)
set OPENAI_API_KEY=your_openai_api_key_here

# Linux/macOS
export OPENAI_API_KEY="your_openai_api_key_here"
```

#### Method 2: appsettings.json (Local development only)
Create an `appsettings.json` file in the project root:
```json
{
  "OpenAI": {
    "ApiKey": "your_openai_api_key_here"
  }
}
```

**Note**: Add `appsettings.json` to your `.gitignore` file to prevent accidental commits.

## Usage

### Running the Application

```bash
dotnet run
```

Or run the compiled executable:

```bash
dotnet run --project ServiceAnalyzer.csproj
```

### Input Options

When prompted, you can provide:

#### Option 1: Known Service Name
Enter the name of a well-known digital service:
```
Input: Spotify
Input: Notion
Input: Discord
Input: Figma
```

#### Option 2: Service Description
Paste or type a description of a service:
```
Input: A platform that helps content creators monetize their audience through subscription-based newsletters and exclusive content, featuring built-in payment processing and analytics tools.
```

### Example Session

```
🔍 Service Analyzer
==================

Enter service information:
📝 Option 1: Service name (e.g., 'Spotify', 'Notion')
📝 Option 2: Service description text

Input: Notion

🤖 Analyzing service... Please wait...

📊 Service Analysis Report
==========================

## Brief History
- Founded in 2016 by Ivan Zhao and Simon Last in San Francisco
- Started as a simple note-taking app and evolved into a comprehensive workspace platform
- Achieved unicorn status in 2021 with a $10 billion valuation
- Rapidly grew from a small team to over 400 employees by 2023

## Target Audience
- Knowledge workers and remote teams seeking organizational tools
- Students and academics who need structured note-taking systems
- Small to medium businesses requiring collaborative project management
- Individual users looking for an all-in-one productivity solution

[... rest of the report ...]

💾 Save report to file? (y/n): y
✅ Report saved to: service_report_20250616_143022.md
```

## Error Handling

The application handles various error scenarios:

- **Missing API Key**: Clear instructions on how to set up your OpenAI API key
- **API Errors**: Detailed error messages for troubleshooting API issues
- **Network Issues**: Helpful error messages for connectivity problems
- **Invalid Input**: Graceful handling of empty or invalid inputs

## Output Files

Generated reports are saved with timestamps in the filename format:
```
service_report_YYYYMMDD_HHMMSS.md
```

Example: `service_report_20250616_143022.md`

## Troubleshooting

### Common Issues

1. **"OpenAI API key not found" Error**
   - Verify your API key is set correctly using one of the configuration methods
   - Check that your API key is valid and has sufficient credits

2. **"OpenAI API error: 401" (Unauthorized)**
   - Your API key is invalid or expired
   - Generate a new API key from the OpenAI Platform

3. **"OpenAI API error: 429" (Rate Limited)**
   - You've exceeded your API rate limits
   - Wait a few minutes before trying again
   - Consider upgrading your OpenAI plan

4. **Network Connection Issues**
   - Check your internet connection
   - Verify you can access https://api.openai.com

### Getting Help

If you encounter issues:

1. Check the error message for specific guidance
2. Verify your OpenAI API key configuration
3. Ensure you have sufficient API credits
4. Check the OpenAI status page for service outages

## Development

### Project Structure

```
ServiceAnalyzer/
├── Program.cs              # Main application logic
├── ServiceAnalyzer.csproj  # Project configuration
├── README.md              # This file
└── sample_outputs.md      # Example outputs
```

### Key Dependencies

- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Extensions.Configuration.Json**: JSON configuration support
- **Microsoft.Extensions.Configuration.EnvironmentVariables**: Environment variable support
- **System.Text.Json**: JSON serialization for API requests
- **HttpClient**: HTTP requests to OpenAI API

## License

This project is for educational purposes as part of the AI Challenge 2025.

## Contributing

This is an educational project. Feel free to fork and experiment!
