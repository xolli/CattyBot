using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CattyBot.handlers.commands;

public class SysPromptDeleteCommand(IServiceScopeFactory scopeFactory) : Command
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

        var inlineMarkup = new InlineKeyboardMarkup();
        foreach (var p in prompts)
        {
            inlineMarkup.AddButton(p.Name, $"sysPromptDelete:{p.Id}");
        }
        inlineMarkup.AddButton("Cancel", "removeMessage");

        await client.SendMessage(
            message.Chat.Id,
            "Select system prompt to delete",
            cancellationToken: cancelToken,
            replyParameters: new ReplyParameters { ChatId = message.Chat.Id, MessageId = message.MessageId },
            replyMarkup: inlineMarkup
        );
    }
}
