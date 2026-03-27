using GenReport.Infrastructure.Models.HttpResponse.Core.Ai;
using System.Collections.Generic;

namespace GenReport.Infrastructure.Models.HttpResponse.Core.Chat
{
    public class ChatProviderModelsResponse
    {
        public string Provider { get; set; } = string.Empty;
        public List<ProviderModelResponse> Models { get; set; } = [];
    }
}
