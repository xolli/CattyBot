using CattyBot.database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.handlers;

public abstract class Handler
{
    public abstract Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken cancelToken,
        Locale language = Locale.RU);
}