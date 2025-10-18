using CattyBot.database;

namespace CattyBot.services;

public class EventService(CattyBotContext db) : BaseService(db)
{
    public List<Event> GetTodayEvents()
    {
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        return (from even in Db.Events
            where even.Enabled && even.Day == now.Day && even.Month == now.Month
            select even).ToList();
    }
}