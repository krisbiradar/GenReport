using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.AI;
using GenReport.Infrastructure.Models.HttpRequests.Core.Chat;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Security.Encryption;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Chat
{
    public class AddMessage(
        ApplicationDbContext context, 
        ICurrentUserService currentUserService,
        IIntentClassifierService intentClassifierService,
        ICredentialEncryptorFactory encryptorFactory) : Endpoint<AddMessageRequest, HttpResponse<ChatMessage>>
    {
        public override void Configure()
        {
            Post("/chat/sessions/messages");
        }

        public override async Task HandleAsync(AddMessageRequest req, CancellationToken ct)
        {
            // The Next.js AI SDK frontend should pass the actual database session ID
            if (!long.TryParse(req.SessionId, out var sessionId))
            {
                await SendAsync(new HttpResponse<ChatMessage>(HttpStatusCode.BadRequest, "Missing or invalid session ID.", "ERR_BAD_REQUEST", []), cancellation: ct);
                return;
            }

            var userId = currentUserService.LoggedInUserId();

            var session = await context.ChatSessions
                .Include(s => s.AiConnection)
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, ct);

            if (session == null)
            {
                await SendAsync(new HttpResponse<ChatMessage>(HttpStatusCode.NotFound, "Chat session not found or access denied.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            // Extract the actual message content from the Next.js AI SDK request payload
            var lastUserMessage = req.Messages.LastOrDefault(m => m.Role == "user");
            if (lastUserMessage == null)
            {
                await SendAsync(new HttpResponse<ChatMessage>(HttpStatusCode.BadRequest, "No user message found to process.", "ERR_BAD_REQUEST", []), cancellation: ct);
                return;
            }

            var role = lastUserMessage.Role;
            var content = lastUserMessage.Content ?? lastUserMessage.Parts?.FirstOrDefault()?.Text ?? string.Empty;

            if (ValidationFailed)
            {
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            // Intent Classification
            string? classifiedIntentName = null;
            ChatIntent? intentEnum = null;
            
            if (session.AiConnection != null)
            {
                var apiKeyEncryptor = encryptorFactory.GetEncryptor(CredentialType.ApiKey);
                var decryptedApiKey = apiKeyEncryptor.Decrypt(session.AiConnection.ApiKey);
                
                var classificationResult = await intentClassifierService.ClassifyAsync(
                    content,
                    session.AiConnection.Provider,
                    decryptedApiKey,
                    session.AiConnection.DefaultModel,
                    ct);

                intentEnum = classificationResult.Intent;
                classifiedIntentName = classificationResult.Intent.ToString();
            }

            var uploadedMediaFiles = new List<GenReport.Domain.Entities.Media.MediaFile>();

            foreach (var attachedFile in req.Attachments)
            {
                var mediaFile = new GenReport.Domain.Entities.Media.MediaFile(
                    storageUrl: attachedFile.Url,
                    fileName: attachedFile.FileName,
                    mimeType: attachedFile.ContentType,
                    size: attachedFile.Size
                );
                
                context.MediaFiles.Add(mediaFile);
                uploadedMediaFiles.Add(mediaFile);
            }

            // Save User Message
            var message = new ChatMessage
            {
                SessionId = sessionId,
                Role = role,
                Content = content,
                Intent = classifiedIntentName,
                Attachments = new List<MessageAttachment>()
            };

            foreach (var mediaFile in uploadedMediaFiles)
            {
                message.Attachments.Add(new MessageAttachment
                {
                    Message = message,
                    MediaFile = mediaFile
                });
            }

            context.ChatMessages.Add(message);
            session.UpdatedAt = DateTime.UtcNow; // Touch session

            // Handle OutOfScope / Sensitive immediately instead of calling main LLM
            if (intentEnum == ChatIntent.OutOfScope || intentEnum == ChatIntent.Sensitive)
            {
                var assistantMessage = new ChatMessage
                {
                    SessionId = sessionId,
                    Role = "assistant",
                    Content = intentEnum == ChatIntent.Sensitive 
                        ? "I cannot process requests involving sensitive information, passwords, credentials, or personal data."
                        : "I'm sorry, I can only help with database queries, report generation, or general questions about my capabilities.",
                    Attachments = new List<MessageAttachment>()
                };
                
                context.ChatMessages.Add(assistantMessage);
                await context.SaveChangesAsync(ct);
                
                // Return the assistant response immediately
                await SendAsync(new HttpResponse<ChatMessage>(assistantMessage, "Processed out of scope request", HttpStatusCode.OK), cancellation: ct);
                return;
            }

            await context.SaveChangesAsync(ct);

            // In typical cases, if not OutOfScope/Sensitive, the UI expects just the user message echoed back 
            // or the backend streams the response. Since this flow returns OK with the user message, we stick to existing behavior
            await SendAsync(new HttpResponse<ChatMessage>(message, "Message added successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
