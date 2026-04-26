using CattyBot.database;
using CattyBot.dto;
using CattyBot.dto.gemini;
using CattyBot.exceptions;
using CattyBot.services;
using CattyBot.utility;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CattyBot.handlers;

public class GeminiHandler(IServiceScopeFactory scopeFactory) : Handler
{
    public const string GeminiApiKeyEnv = "GOOGLE_API_KEY";

    private readonly GeminiBot _geminiBot = new();

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
            LogAnalytics(chatId, "gemini-2.5-flash", "Google API", systemPromptId);

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
        catch (GeminiException ex)
        {
            Log.Error(ex, "Can't use Gemini API");
            Log.Error(ex.Message);
            await client.SendMessage(
                chatId,
                "Не могу придумать ответ. Напиши ещё раз",
                cancellationToken: cancelToken,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                replyParameters: new ReplyParameters { ChatId = chatId, MessageId = update.Message.MessageId }
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Cannot send telegram message");
            await client.SendMessage(
                chatId,
                "Неожиданная ошибка. Напиши ещё раз",
                cancellationToken: cancelToken,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                replyParameters: new ReplyParameters { ChatId = chatId, MessageId = update.Message.MessageId }
            );
        }
    }

    private async Task<string> GenerateResponse(long chatId, List<MessageContent> formattedMessages, int? systemPromptId,
        CancellationToken cancelToken, string? systemInstruction)
    {
        var contents = GetHistory(chatId, systemPromptId)
            .Concat(formattedMessages.Select(content =>
                {
                    List<GeminiContent> contents = [];
                    if (content.text is not null)
                    {
                        contents.Add(new GeminiContent(content.text, null));
                    }
                    if (content.photoBase64 is not null)
                    {
                        contents.Add(new GeminiContent(null, new GeminiInlineData("image/jpeg", content.photoBase64)));
                    }
                    return new GeminiMessage(contents, "user");
                }
            ).Where(msg => msg.parts.Count > 0))
            .ToList();
        return await _geminiBot.GenerateTextResponse(contents, "gemini-2.5-flash", cancelToken, systemInstruction);
    }

    private List<GeminiMessage> GetHistory(long chatId, int? systemPromptId)
    {
        using var messageServiceScope = scopeFactory.CreateScope();
        var messageService = messageServiceScope.ServiceProvider.GetRequiredService<MessageService>();
        return messageService.GetPreviousMessagesGemini(chatId, 25, systemPromptId);
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

    private void LogAnalytics(long chatId, string model, string provider, int? systemPromptId)
    {
        using var scope = scopeFactory.CreateScope();
        var analyticsService = scope.ServiceProvider.GetRequiredService<AnalyticsService>();
        analyticsService.LogAnalytics(chatId, model, provider);
    }
}