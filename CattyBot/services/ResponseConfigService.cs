using CattyBot.database;
using Microsoft.EntityFrameworkCore;

namespace CattyBot.services;

public class ResponseConfigService(CattyBotContext db) : BaseService(db)
{
    public ResponseConfig GetResponseConfig(long chatId)
    {
        var config = GetChatConfig(chatId);

        if (config != null) return config;
        var defaultConfig = new ResponseConfig
            { ChatId = chatId, HelloMessage = true, ChatBot = true, SystemPromptId = null };
        Db.Add(defaultConfig);
        Db.SaveChanges();
        return defaultConfig;

    }

    public bool ReverseHelloMessageStatus(long chatId)
    {
        var config = GetChatConfig(chatId);

        if (config == null)
        {
            var newConfig = new ResponseConfig
                { ChatId = chatId, HelloMessage = false, ChatBot = true, SystemPromptId = null };
            Db.Add(newConfig);
            Db.SaveChanges();
            return false;
        }

        config.HelloMessage = !config.HelloMessage;
        Db.SaveChanges();
        return config.HelloMessage;
    }

    public bool ReverseChatBotStatus(long chatId)
    {
        var config = GetChatConfig(chatId);

        if (config == null)
        {
            var newConfig = new ResponseConfig
                { ChatId = chatId, HelloMessage = true, ChatBot = false, SystemPromptId = null };
            Db.Add(newConfig);
            Db.SaveChanges();
            return false;
        }

        config.ChatBot = !config.ChatBot;
        Db.SaveChanges();
        return config.ChatBot;
    }

    public int? GetSystemPromptId(long chatId)
    {
        var config = GetChatConfig(chatId);
        if (config != null) return config.SystemPromptId;
        var newConfig = new ResponseConfig
            { ChatId = chatId, HelloMessage = true, ChatBot = false, SystemPromptId = null };
        Db.Add(newConfig);
        Db.SaveChanges();
        return null;
    }

    public void SetSystemPromptId(long chatId, int? systemPromptId)
    {
        var config = GetChatConfig(chatId);
        if (config == null)
            Db.Add(new ResponseConfig { ChatId = chatId, HelloMessage = true, ChatBot = false, SystemPromptId = systemPromptId });
        else
            config.SystemPromptId = systemPromptId;
        Db.SaveChanges();
    }

    private ResponseConfig? GetChatConfig(long chatId)
    {
        return Db.ResponseConfigs
            .Include(c => c.SystemPrompt)
            .FirstOrDefault(c => c.ChatId == chatId);
    }
}