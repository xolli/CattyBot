using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CattyBot.handlers.commands;

public class SetModeCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message,
        CancellationToken cancelToken)
    {
        if (message.From == null) return;
        var chatId = message.Chat.Id;
        using var scope = scopeFactory.CreateScope();
        var responseConfigService = scope.ServiceProvider.GetRequiredService<ResponseConfigService>();
        var systemPromptService = scope.ServiceProvider.GetRequiredService<SystemPromptService>();
        var responseConfig = responseConfigService.GetResponseConfig(chatId);
        var currentPromptId = responseConfig.SystemPromptId;
        var prompts = systemPromptService.GetAll();

        var inlineMarkup = new InlineKeyboardMarkup();
        foreach (var prompt in prompts.Where(prompt => prompt.Id != currentPromptId))
            inlineMarkup.AddButton(prompt.Name, $"setSysPrompt:{prompt.Id}");

        var messageText = "Промпт не установлен\\. Выберите новый из списка";
        if (currentPromptId is not null)
        {
            inlineMarkup.AddButton("Без промпта", "setSysPrompt:null");
            messageText = $"Текущий режим: *{responseConfig.SystemPrompt?.Name}*\nВыберите новый из списка";
        }

        inlineMarkup.AddButton("Отмена", "removeMessage");

        await client.SendMessage(
            chatId,
            messageText,
            cancellationToken: cancelToken,
            parseMode: ParseMode.MarkdownV2,
            replyParameters: new ReplyParameters { ChatId = chatId, MessageId = message.MessageId },
            replyMarkup: inlineMarkup
        );
    }
}