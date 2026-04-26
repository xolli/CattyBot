using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.handlers.commands;

public class SysPromptListCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();
        var prompts = service.GetAll();

        if (prompts.Count == 0)
        {
            await client.SendMessage(message.Chat.Id, "No system prompts", cancellationToken: cancelToken);
            return;
        }

        var lines = prompts.Select(p => $"{p.Id} | {p.Name}").ToList();
        await client.SendMessage(
            message.Chat.Id,
            string.Join("\n", lines),
            cancellationToken: cancelToken
        );
    }
}
