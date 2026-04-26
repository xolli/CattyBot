using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CattyBot.handlers.commands;

public class SysPromptUpdateCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        if (message.From == null || message.Text == null) return;

        var text = message.Text;
        var newlineIndex = text.IndexOf('\n');
        if (newlineIndex < 0)
        {
            await client.SendMessage(
                message.Chat.Id,
                "Usage: \n`/prompt_update` \\<id\\>\n\\<new prompt text\\>",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancelToken
            );
            return;
        }

        var firstLine = text.Substring(0, newlineIndex).Trim();
        var parts = ParseCommand(firstLine);
        if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
        {
            await client.SendMessage(
                message.Chat.Id,
                "Usage: \n`/prompt_update` \\<id\\>\n\\<new prompt text\\>",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancelToken
            );
            return;
        }

        var content = text[(newlineIndex + 1)..].Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            await client.SendMessage(message.Chat.Id, "Prompt text is required", cancellationToken: cancelToken);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();

        var updated = service.UpdateContent(id, content);
        await client.SendMessage(
            message.Chat.Id,
            updated ? $"System prompt updated: {id}" : $"System prompt not found: {id}",
            cancellationToken: cancelToken
        );
    }
}
