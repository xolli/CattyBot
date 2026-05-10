using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CattyBot.handlers.commands;

public class ModeRemoveCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        if (message.Chat == null) return;

        var text = message.Text;
        var parts = ParseCommand(text ?? "");
        
        if (parts.Length > 1)
        {
            // Delete by name if provided
            var name = string.Join(' ', parts.Skip(1)).Trim();
            await DeleteByName(client, message, name, cancelToken);
            return;
        }

        // Show interactive list if no name provided
        await ShowDeleteList(client, message, cancelToken);
    }

    private async Task DeleteByName(ITelegramBotClient client, Message message, string name, CancellationToken cancelToken)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();

        try
        {
            var success = service.DeleteByName(name, message.Chat.Id);
            if (success)
            {
                await client.SendMessage(
                    message.Chat.Id,
                    $"User prompt '{name}' deleted successfully",
                    cancellationToken: cancelToken
                );
            }
            else
            {
                await client.SendMessage(
                    message.Chat.Id,
                    $"User prompt '{name}' not found in this chat",
                    cancellationToken: cancelToken
                );
            }
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

    private async Task ShowDeleteList(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SystemPromptService>();
        var prompts = service.GetUserPromptsForChat(message.Chat.Id);

        if (prompts.Count == 0)
        {
            await client.SendMessage(
                message.Chat.Id,
                "No user-defined prompts in this chat",
                cancellationToken: cancelToken
            );
            return;
        }

        var inlineMarkup = new InlineKeyboardMarkup();
        foreach (var p in prompts)
        {
            inlineMarkup.AddButton(p.Name, $"modeDelete:{p.Id}");
        }
        inlineMarkup.AddButton("Cancel", "removeMessage");

        await client.SendMessage(
            message.Chat.Id,
            "Промпт для удаления",
            cancellationToken: cancelToken,
            replyParameters: new ReplyParameters { ChatId = message.Chat.Id, MessageId = message.MessageId },
            replyMarkup: inlineMarkup
        );
    }
}
