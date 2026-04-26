using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CattyBot.database;

public class ResponseConfig
{
    [Key] public int Id { get; set; }

    [Required] public long ChatId { get; set; }

    [Required] public bool HelloMessage { get; set; }

    [Required] public bool ChatBot { get; set; }

    public int? SystemPromptId { get; set; }

    [ForeignKey("SystemPromptId")]
    public virtual SystemPrompt? SystemPrompt { get; set; }
}
