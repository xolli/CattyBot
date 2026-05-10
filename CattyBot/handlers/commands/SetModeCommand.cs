using CattyBot.database;
using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CattyBot.handlers.commands;

public class SetModeCommand(IServiceScopeFactory scopeFactory) : Command
{
    private const int ButtonsPerRow = 2;
    public const string CallbackPrefix = "setSysPrompt:";

    protected override async Task HandleCommand(ITelegramBotClient client, Message message, CancellationToken cancelToken)
    {
        if (message.From == null) return;
        var chatId = message.Chat.Id;

        using var scope = scopeFactory.CreateScope();
        var responseConfigService = scope.ServiceProvider.GetRequiredService<ResponseConfigService>();
        var systemPromptService = scope.ServiceProvider.GetRequiredService<SystemPromptService>();

        var responseConfig = responseConfigService.GetResponseConfig(chatId);
        var currentPromptId = responseConfig.SystemPromptId;
        var allPrompts = systemPromptService.GetAllForChat(chatId);

        var buttons = allPrompts
            .Where(p => p.Id != currentPromptId)
            .Select(p => InlineKeyboardButton.WithCallbackData(
                $"{GetPromptEmoji(p.Type)} {p.Name}", 
                $"{CallbackPrefix}{p.Id}"))
            .ToList();

        if (currentPromptId is not null)
            buttons.Add(InlineKeyboardButton.WithCallbackData("❌ Без промпта", $"{CallbackPrefix}null"));
        
        buttons.Add(InlineKeyboardButton.WithCallbackData("🚫 Отмена", "removeMessage"));

        var inlineMarkup = new InlineKeyboardMarkup(BuildRows(buttons, ButtonsPerRow));

        var currentName = responseConfig.SystemPrompt?.Name ?? "не установлен";
        var messageText = currentPromptId is null 
            ? "Промпт не установлен\\. Выберите новый из списка"
            : $"Текущий режим: *{currentName}*\nВыберите новый из списка\n\n" +
              "🔧 \\- Глобальные промпты\n👥 \\- Пользовательские промпты";

        await client.SendMessage(
            chatId,
            messageText,
            parseMode: ParseMode.MarkdownV2,
            replyParameters: new ReplyParameters { ChatId = chatId, MessageId = message.MessageId },
            replyMarkup: inlineMarkup,
            cancellationToken: cancelToken
        );
    }

    private static string GetPromptEmoji(PromptType type) => type switch
    {
        PromptType.Admin => "🔧",
        PromptType.User => "👥",
        _ => "📝"
    };

    private static IEnumerable<IEnumerable<InlineKeyboardButton>> BuildRows(List<InlineKeyboardButton> buttons, int chunkSize)
    {
        for (int i = 0; i < buttons.Count; i += chunkSize)
        {
            yield return buttons.Skip(i).Take(chunkSize);
        }
    }
}
