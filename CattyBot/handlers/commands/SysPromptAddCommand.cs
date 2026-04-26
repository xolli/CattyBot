using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CattyBot.handlers.commands;

public class SysPromptAddCommand(IServiceScopeFactory scopeFactory) : Command
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
                "Usage: `/prompt_add` \\<name\\>\n\\<prompt text\\>",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancelToken
            );
            return;
        }

        var firstLine = text.Substring(0, newlineIndex).Trim();
        var parts = ParseCommand(firstLine);
        if (parts.Length < 2)
        {
            await client.SendMessage(
                message.Chat.Id,
                "Usage: /prompt_add <name>\\n<prompt text>",
                cancellationToken: cancelToken
            );
            return;
        }

        var name = string.Join(' ', parts.Skip(1)).Trim();
        var content = text[(newlineIndex + 1)..].Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
        {
            await client.SendMessage(
                message.Chat.Id,
                "Name and prompt text are required",
                cancellationToken: cancelToken
            );
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();

        try
        {
            var prompt = service.Create(name, content);
            await client.SendMessage(
                message.Chat.Id,
                $"System prompt added: {prompt.Id} | {prompt.Name}",
                cancellationToken: cancelToken
            );
        }
        catch (Exception ex)
        {
            await client.SendMessage(
                message.Chat.Id,
                $"Error: {ex.Message}",
                cancellationToken: cancelToken
            );
        }
    }
}
