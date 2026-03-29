using GenReport.DB.Domain.Entities.Core;

using Microsoft.EntityFrameworkCore;

namespace GenReport.DB.Domain.Seed
{
    public partial class ApplicationDBContextSeeder
    {
        private const string IntentClassifierSystemPrompt = @"You are an intent classifier for a database reporting assistant called GenReport.
Classify the user's message into exactly one of these intents:

- Greeting: casual greetings like hi, hello, how are you, good morning, hey
- BotInfo: questions about the bot itself — who are you, what can you do, help, about
- DatabaseQuery: requests about database information — current size, top tables, stored procedures, schemas, column info, table structure, row counts, indexes
- ReportGenerate: requests to generate reports, create summaries, export data, build dashboards
- Sensitive: any request for sensitive information such as passwords, credentials, API keys, connection strings, secrets, tokens, or personally identifiable information (PII) like emails, phone numbers, addresses
- OutOfScope: anything that does not fit any of the above categories

IMPORTANT: If a message asks for sensitive data (passwords, credentials, PII), always classify as Sensitive, NOT OutOfScope.

Respond with ONLY a JSON object matching this exact schema, no other text:
{""intent"": ""<one of: Greeting, BotInfo, DatabaseQuery, ReportGenerate, Sensitive, OutOfScope>"", ""confidence"": <number between 0.0 and 1.0>}";

        public async Task SeedAiConfigs()
        {
            var now = DateTime.UtcNow;

            // Find the seeded Gemini connection
            var geminiConnection = await applicationDbContext.AiConnections
                .FirstOrDefaultAsync(c => c.Provider == "gemini");

            if (geminiConnection == null)
            {
                return; // Nothing to attach to, wait until connections are seeded
            }

            var defaultGeminiModel = geminiConnection.DefaultModel;
            var lightweightModel = defaultGeminiModel; // Use default unless we specifically map it
            if (geminiConnection.Provider == "gemini") {
                lightweightModel = "gemini-2.0-flash-lite";
            }

            // Check if we already have an active IntentClassifier for Gemini with this specific model
            var hasConfig = await applicationDbContext.AiConfigs
                .AnyAsync(c => c.Type == AiConfigType.IntentClassifier 
                            && c.AiConnectionId == geminiConnection.Id 
                            && c.ModelId == lightweightModel
                            && c.IsActive);

            if (!hasConfig)
            {
                var config = new AiConfig
                {
                    Type = AiConfigType.IntentClassifier,
                    Value = IntentClassifierSystemPrompt,
                    AiConnectionId = geminiConnection.Id,
                    ModelId = lightweightModel,
                    IsActive = true,
                    Version = 1,
                    CreatedAt = now,
                    UpdatedAt = now,
                };

                await applicationDbContext.AiConfigs.AddAsync(config);
                await applicationDbContext.SaveChangesAsync();
                logger.Information("Seeded IntentClassifier AiConfig for Gemini default connection.");
            }
        }
    }
}
