using CattyBot.services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CattyBot.handlers.commands;

public class RemoveBirthday(IServiceScopeFactory scopeFactory) : Command
{
    protected override async Task HandleCommand(ITelegramBotClient client, Message message,
        CancellationToken cancelToken)
    {
        if (message.From == null) return;
        var chatId = message.Chat.Id;

        var scope = scopeFactory.CreateScope();
        var birthdaysService = scope.ServiceProvider.GetRequiredService<BirthdaysService>();
        if (birthdaysService.RemoveBirthday(message.From))
            await client.SendMessage(
                chatId,
                "Пока что не буду поздравлять тебя с днём рождения 👌",
                cancellationToken: cancelToken);
        else
            await client.SendMessage(
                chatId,
                "Я и так не знаю когда у тебя день рождения 😅",
                cancellationToken: cancelToken);
    }
}