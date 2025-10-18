using CattyBot.database;
using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CattyBot.handlers.commands;

public class ClearContextCommand(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message,
        CancellationToken cancelToken)
    {
        var scope = scopeFactory.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
        var chatId = message.Chat.Id;
        var responseConfigService = scope.ServiceProvider.GetRequiredService<ResponseConfigService>();
        var currentMode = responseConfigService.GetChatMode(chatId);
        messageService.ClearChatMessages(chatId, currentMode);
        await client.SendMessage(
            message.Chat.Id,
            $"История общения со мной очищена \\(в рамках режима *{Localizer.GetValue(currentMode.ToString(), Locale.RU)}*\\) 👌",
            cancellationToken: cancelToken,
            parseMode: ParseMode.MarkdownV2,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            replyParameters: new ReplyParameters { ChatId = chatId, MessageId = message.MessageId }
        );
    }
}