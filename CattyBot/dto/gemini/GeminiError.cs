namespace CattyBot.dto.gemini;

public record GeminiError(int code, string message, string status);