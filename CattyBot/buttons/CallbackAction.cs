using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.buttons;

public interface ICallbackAction
{
    public void Handle(ITelegramBotClient client, CallbackQuery callback, CancellationToken cancelToken);
}