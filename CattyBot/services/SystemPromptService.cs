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

    public SystemPrompt? GetById(int id)
    {
        return (from p in Db.SystemPrompts where p.Id == id select p).FirstOrDefault();
    }

    public SystemPrompt? GetByName(string name)
    {
        return (from p in Db.SystemPrompts where p.Name == name select p).FirstOrDefault();
    }

    public string? GetContentByName(string name)
    {
        return (from p in Db.SystemPrompts where p.Name == name select p.Content).FirstOrDefault();
    }

    public SystemPrompt Create(string name, string content)
    {
        var existing = GetByName(name);
        if (existing is not null)
            throw new InvalidOperationException($"System prompt with name '{name}' already exists");

        var prompt = new SystemPrompt
        {
            Name = name,
            Content = content
        };
        Db.SystemPrompts.Add(prompt);
        Db.SaveChanges();
        return prompt;
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
}
