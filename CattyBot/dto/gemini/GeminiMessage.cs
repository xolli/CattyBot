namespace CattyBot.dto.gemini;

public record GeminiMessage(List<GeminiContent> parts, string? role);