using CattyBot.database;
using CattyBot.dto;
using CattyBot.services;
using CattyBot.utility;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace CattyBot.handlers;

public class OpenRouterHandler(IServiceScopeFactory scopeFactory) : Handler
{
    private readonly OpenRouterBot _bot = new();

    public override async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken cancelToken,
        Locale language = Locale.RU)
    {
        await GenerateResponse(client, update, null, cancelToken, language);
    }

    public async Task GenerateResponse(ITelegramBotClient client, Update update, long? myId,
        CancellationToken cancelToken, Locale language = Locale.RU)
    {
        if (update.Message is null) return;
        var formattedMessages = await Util.FormatMessageWithReplies(client, update.Message, myId, cancelToken);
        Log.Debug($"Formatted message: {string.Join(", ", formattedMessages.Select(x => x.text ?? ""))}");
        var chatId = update.Message.Chat.Id;
        try
        {
            client.SendChatAction(
                chatId,
                ChatAction.Typing,
                cancellationToken: cancelToken
            );
            using var responseConfigServiceScope = scopeFactory.CreateScope();
            var responseConfigService =
                responseConfigServiceScope.ServiceProvider.GetRequiredService<ResponseConfigService>();
            var responseConfig = responseConfigService.GetResponseConfig(chatId);
            var systemPromptId = responseConfig.SystemPromptId;
            var systemInstruction = responseConfig.SystemPrompt?.Content;
            var messageContent =
                await GenerateResponse(update.Message.Chat.Id, formattedMessages, systemPromptId, cancelToken, systemInstruction);
            LogHistoryMessages(formattedMessages, messageContent, update.Message.Chat.Id, systemPromptId);

            var chunks = StringUtils.SplitTextIntoChunks(messageContent);
            foreach (var chunk in chunks)
            {
                await client.SendMessage(
                    chatId,
                    chunk,
                    cancellationToken: cancelToken,
                    linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                    replyParameters: new ReplyParameters { ChatId = chatId, MessageId = update.Message.MessageId }
                );
                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Can't use OpenRouter API");
            await client.SendMessage(
                chatId,
                "Что-то пошло не так. Напиши ещё раз",
                cancellationToken: cancelToken,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                replyParameters: new ReplyParameters { ChatId = chatId, MessageId = update.Message.MessageId }
            );
        }
    }

    private async Task<string> GenerateResponse(long chatId, List<MessageContent> formattedMessages, int? systemPromptId,
        CancellationToken cancelToken, string? systemInstruction)
    {
        var history = GetHistory(chatId, systemPromptId);
        var userMessages = formattedMessages
            .Select(content =>
            {
                var parts = new List<ChatMessageContentPart>();
                if (content.text is not null)
                    parts.Add(ChatMessageContentPart.CreateTextPart(content.text));
                if (content.photoBase64 is not null)
                    parts.Add(ChatMessageContentPart.CreateImagePart(
                        BinaryData.FromBytes(Convert.FromBase64String(content.photoBase64)), "image/jpeg"));
                return parts.Count > 0 ? ChatMessage.CreateUserMessage(parts) : null;
            })
            .Where(msg => msg is not null)
            .Select(msg => msg!)
            .ToList();

        var messages = new List<ChatMessage>();
        if (systemInstruction is not null)
            messages.Add(ChatMessage.CreateSystemMessage(systemInstruction));
        messages.AddRange(history);
        messages.AddRange(userMessages);

        return await _bot.GenerateTextResponse(messages, cancelToken);
    }

    private List<ChatMessage> GetHistory(long chatId, int? systemPromptId)
    {
        using var messageServiceScope = scopeFactory.CreateScope();
        var messageService = messageServiceScope.ServiceProvider.GetRequiredService<MessageService>();
        return messageService.GetPreviousMessagesOpenRouter(chatId, 25, systemPromptId);
    }

    private void LogHistoryMessages(List<MessageContent> userMessages, string botResponse, long chatId, int? systemPromptId)
    {
        using var messageServiceScope = scopeFactory.CreateScope();
        var messageService = messageServiceScope.ServiceProvider.GetRequiredService<MessageService>();
        userMessages.ForEach(msg =>
        {
            if (msg.text != null)
                messageService.LogMessage(new HistoricalMessage
                    { Content = msg.text, ChatId = chatId, IsBot = false, SystemPromptId = systemPromptId });
        });
        messageService.LogMessage(new HistoricalMessage
            { Content = botResponse, ChatId = chatId, IsBot = true, SystemPromptId = systemPromptId });
    }
}