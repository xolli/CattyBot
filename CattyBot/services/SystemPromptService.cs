using CattyBot.database;

namespace CattyBot.services;

public class SystemPromptService(CattyBotContext db) : BaseService(db)
{
    public List<SystemPrompt> GetAll()
    {
        return Db.SystemPrompts
            .OrderBy(p => p.Name)
            .ToList();
    }

    public List<SystemPrompt> GetAdminPrompts()
    {
        return Db.SystemPrompts
            .Where(p => p.Type == PromptType.Admin)
            .OrderBy(p => p.Name)
            .ToList();
    }

    public List<SystemPrompt> GetUserPromptsForChat(long chatId)
    {
        return Db.SystemPrompts
            .Where(p => p.Type == PromptType.User && p.ChatId == chatId)
            .OrderBy(p => p.Name)
            .ToList();
    }

    public List<SystemPrompt> GetAllForChat(long chatId)
    {
        return Db.SystemPrompts
            .Where(p => p.Type == PromptType.Admin || (p.Type == PromptType.User && p.ChatId == chatId))
            .OrderBy(p => p.Type) // Admin prompts first
            .ThenBy(p => p.Name)
            .ToList();
    }

    public SystemPrompt? GetById(int id)
    {
        return (from p in Db.SystemPrompts where p.Id == id select p).FirstOrDefault();
    }

    public SystemPrompt? GetByName(string name)
    {
        return (from p in Db.SystemPrompts where p.Name == name select p).FirstOrDefault();
    }

    public SystemPrompt? GetByNameAndChat(string name, long chatId)
    {
        return (from p in Db.SystemPrompts where p.Name == name && p.ChatId == chatId select p).FirstOrDefault();
    }

    public string? GetContentByName(string name)
    {
        return (from p in Db.SystemPrompts where p.Name == name select p.Content).FirstOrDefault();
    }

    public string? GetContentByNameAndChat(string name, long chatId)
    {
        return (from p in Db.SystemPrompts where p.Name == name && p.ChatId == chatId select p.Content).FirstOrDefault();
    }

    public SystemPrompt Create(string name, string content, PromptType type = PromptType.Admin, long? chatId = null)
    {
        if (type == PromptType.User && chatId == null)
            throw new ArgumentException("ChatId is required for user prompts");

        SystemPrompt? existing;
        if (type == PromptType.User && chatId.HasValue)
            existing = GetByNameAndChat(name, chatId.Value);
        else
            existing = GetByName(name);

        if (existing is not null)
            throw new InvalidOperationException($"System prompt with name '{name}' already exists");

        var prompt = new SystemPrompt
        {
            Name = name,
            Content = content,
            Type = type,
            ChatId = chatId
        };
        Db.SystemPrompts.Add(prompt);
        Db.SaveChanges();
        return prompt;
    }

    public SystemPrompt CreateUserPrompt(string name, string content, long chatId)
    {
        return Create(name, content, PromptType.User, chatId);
    }

    public bool UpdateContent(int id, string content)
    {
        var prompt = GetById(id);
        if (prompt is null) return false;

        prompt.Content = content;
        Db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var prompt = GetById(id);
        if (prompt is null) return false;
        Db.SystemPrompts.Remove(prompt);
        Db.SaveChanges();
        return true;
    }

    public bool Delete(int id, long chatId)
    {
        var prompt = Db.SystemPrompts.FirstOrDefault(p => p.Id == id && p.ChatId == chatId);
        if (prompt is null) return false;
        Db.SystemPrompts.Remove(prompt);
        Db.SaveChanges();
        return true;
    }

    public bool DeleteByName(string name, long chatId)
    {
        var prompt = GetByNameAndChat(name, chatId);
        if (prompt is null) return false;
        Db.SystemPrompts.Remove(prompt);
        Db.SaveChanges();
        return true;
    }
}
