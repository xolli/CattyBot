using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CattyBot.buttons;

public class SysPromptEditAction : ICallbackAction
{
    private const string Prefix = "sysPromptEdit:";

    public void Handle(ITelegramBotClient client, CallbackQuery callback, CancellationToken cancelToken)
    {
        if (callback.Message == null || callback.Data == null) return;
        if (!callback.Data.StartsWith(Prefix)) return;

        if (!int.TryParse(callback.Data[Prefix.Length..], out var id)) return;

        client.SendMessage(
            callback.Message.Chat.Id,
            "Usage: \n`/prompt_update` \\<id\\>\n\\<new prompt text\\>",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancelToken
        );

        client.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId, cancelToken);
    }
}
