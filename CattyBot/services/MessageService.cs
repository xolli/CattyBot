using CattyBot.database;
using CattyBot.dto.gemini;
using OpenAI;
using OpenAI.Chat;

namespace CattyBot.services;

public class MessageService(CattyBotContext db) : BaseService(db)
{
    public List<Message> GetPreviousMessages(long chatId, int limit)
    {
        var messages =
            from msg in Db.Messages
            orderby msg.Id
            where msg.ChatId == chatId
            select msg;
        return messages
            .Skip(Math.Max(0, messages.Count() - limit))
            .Select(message => new Message(message.IsBot ? Role.Assistant : Role.User, message.Content, null))
            .ToList();
    }

    public List<GeminiMessage> GetPreviousMessagesGemini(long chatId, int limit, int? systemPromptId)
    {
        var messages =
            from msg in Db.Messages
            orderby msg.Id
            where msg.ChatId == chatId && msg.SystemPromptId == systemPromptId
            select msg;
        return messages
            .Skip(Math.Max(0, messages.Count() - limit))
            .Select(message => new GeminiMessage(new List<GeminiContent> { new(message.Content, null) },
                message.IsBot ? "model" : "user"))
            .ToList();
    }

    public void ClearChatMessages(long chatId, int? systemPromptId)
    {
        var chatMessages = Db.Messages.Where(m => m.ChatId == chatId && m.SystemPromptId == systemPromptId).ToList();
        Db.Messages.RemoveRange(chatMessages);
        Db.SaveChanges();
    }

    public void LogMessage(HistoricalMessage newMessage)
    {
        Db.Messages.Add(newMessage);
        Db.SaveChanges();
    }
}