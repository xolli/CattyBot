using System.ComponentModel.DataAnnotations;

namespace CattyBot.database;

public class SystemPrompt
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public string Content { get; set; } = null!;
}
