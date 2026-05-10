using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.handlers.commands;

public class ModeListCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        if (message.Chat == null) return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();
        var userPrompts = service.GetUserPromptsForChat(message.Chat.Id);

        if (userPrompts.Count == 0)
        {
            await client.SendMessage(
                message.Chat.Id,
                "No user-defined prompts in this chat",
                cancellationToken: cancelToken
            );
            return;
        }

        var response = new List<string> { "👥 Промпты:" };
        foreach (var p in userPrompts)
        {
            response.Add($"{p.Id} | {p.Name}");
        }

        await client.SendMessage(
            message.Chat.Id,
            string.Join("\n", response),
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancelToken
        );
    }
}
