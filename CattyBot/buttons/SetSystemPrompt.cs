using CattyBot.services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.buttons;

public class SetSystemPrompt(ResponseConfigService responseConfigService) : ICallbackAction
{
    private const string Prefix = "setSysPrompt:"; // FIXME DRY

    public void Handle(ITelegramBotClient client, CallbackQuery callback, CancellationToken cancelToken)
    {
        if (callback.Message == null || callback.Data == null) return;
        if (!callback.Data.StartsWith(Prefix)) return;

        var idStr = callback.Data[Prefix.Length..];
        int? promptId = null;
        if (idStr != "null")
        {
            if (!int.TryParse(idStr, out var parsed)) return;
            promptId = parsed;
        }

        responseConfigService.SetSystemPromptId(callback.Message.Chat.Id, promptId);
        client.SendMessage(
            callback.Message.Chat.Id,
            "Режим изменён",
            cancellationToken: cancelToken
        );
        // FIXME: Do we need it if we remove message?
        // client.AnswerCallbackQuery(callback.Id, cancellationToken: cancelToken);
        client.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId, cancelToken);
    }
}
