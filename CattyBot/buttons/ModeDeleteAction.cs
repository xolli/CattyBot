using CattyBot.services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.buttons;

public class ModeDeleteAction(SystemPromptService systemPromptService) : ICallbackAction
{
    private const string Prefix = "modeDelete:";

    public void Handle(ITelegramBotClient client, CallbackQuery callback, CancellationToken cancelToken)
    {
        if (callback.Message == null || callback.Data == null) return;
        if (!callback.Data.StartsWith(Prefix)) return;

        if (!int.TryParse(callback.Data[Prefix.Length..], out var id)) return;

        var deleted = systemPromptService.Delete(id, callback.Message.Chat.Id);
        client.SendMessage(
            callback.Message.Chat.Id,
            deleted ? $"Промпт удален: {id}" : $"Промпт не найден: {id}",
            cancellationToken: cancelToken
        );

        client.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId, cancelToken);
    }
}
