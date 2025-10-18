using CattyBot.database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.handlers;

public class ReactionHandler : Handler
{
    private static readonly Dictionary<string, string> KeywordsReactionsSubstring = new()
    {
        { "catty", "❤" }, { "аллах", "🙏" }, { "коран", "🙏" }, { "бог", "🙏" }, { "иисус", "🙏" }
    };

    public override async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken cancelToken,
        Locale language = Locale.RU)
    {
        if (update.Message?.Text == null) return;

        foreach (var keyValue in KeywordsReactionsSubstring.Where(keyValue =>
                     update.Message.Text.Contains(keyValue.Key, StringComparison.CurrentCultureIgnoreCase)))
        {
            await client.SetMessageReaction(
                new ChatId(update.Message.Chat.Id),
                update.Message.MessageId,
                new List<ReactionType> { keyValue.Value },
                false,
                cancelToken
            );
            return;
        }
    }
}