using CattyBot.database;

namespace CattyBot.services;

public abstract class BaseService(CattyBotContext db)
{
    protected readonly CattyBotContext Db = db;

    protected User GetOrCreateDbUser(Telegram.Bot.Types.User tgUser)
    {
        var user = (from u in Db.Users where u.UserId == tgUser.Id select u).FirstOrDefault() ?? Db.Users.Add(new User
        {
            UserId = tgUser.Id, Username = tgUser.Username, FirstName = tgUser.FirstName, LastName = tgUser.LastName
        }).Entity;
        return user;
    }
}