using CattyBot.services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.buttons;

public class SysPromptDeleteAction(SystemPromptService systemPromptService) : ICallbackAction
{
    private const string Prefix = "sysPromptDelete:";

    public void Handle(ITelegramBotClient client, CallbackQuery callback, CancellationToken cancelToken)
    {
        if (callback.Message == null || callback.Data == null) return;
        if (!callback.Data.StartsWith(Prefix)) return;

        if (!int.TryParse(callback.Data[Prefix.Length..], out var id)) return;

        var deleted = systemPromptService.Delete(id);
        client.SendMessage(
            callback.Message.Chat.Id,
            deleted ? $"System prompt deleted: {id}" : $"System prompt not found: {id}",
            cancellationToken: cancelToken
        );

        client.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId, cancelToken);
    }
}
