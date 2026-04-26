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
        using var scope = scopeFactory.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
        var responseConfigService = scope.ServiceProvider.GetRequiredService<ResponseConfigService>();
        var chatId = message.Chat.Id;
        var systemPromptId = responseConfigService.GetSystemPromptId(chatId);
        messageService.ClearChatMessages(chatId, systemPromptId);
        await client.SendMessage(
            message.Chat.Id,
            "История общения со мной очищена", // TODO add the system prompt name
            cancellationToken: cancelToken,
            parseMode: ParseMode.MarkdownV2,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            replyParameters: new ReplyParameters { ChatId = chatId, MessageId = message.MessageId }
        );
    }
}