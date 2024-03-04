using OpenAI_API;
using OpenAI_API.Chat;

namespace AIBitrix.Controllers;

public static class AIController
{
    public static OpenAIAPI _client;

    public static Conversation _chat;

    public static void Init()
    {
        _client = new OpenAIAPI("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");

        _chat = _client.Chat.CreateConversation();
            
        var systemString = System.IO.File.ReadAllText(@"./start.txt");
            
        _chat.RequestParameters.Temperature = 0.85;
        _chat.Model.ModelID = "ft:gpt-3.5-turbo-0613:alex-maison::8pPWLFWM";
        _chat.RequestParameters.MaxTokens = 400;
            
        _chat.AppendSystemMessage(systemString);
    }

    public static async Task<string> Generate(string? message)
    {
        _chat.AppendUserInput(message);
            
        var response = await _chat.GetResponseFromChatbotAsync();

        _chat.Messages.Remove(new ChatMessage(ChatMessageRole.User, message));
        return response;
    }
}