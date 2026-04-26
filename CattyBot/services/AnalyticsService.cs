using CattyBot.database;

namespace CattyBot.services;

public class AnalyticsService(CattyBotContext db) : BaseService(db)
{
    public void LogAnalytics(long chatId, string model, string provider)
    {
        var currentAnalytics = (from analytic in Db.ModelsAnalytics
                where analytic.ChatId == chatId && analytic.Model == model && analytic.Provider == provider
                select analytic)
            .FirstOrDefault();
        if (currentAnalytics == null)
            Db.ModelsAnalytics.Add(new ModelAnalytic
                { ChatId = chatId, Model = model, Provider = provider, CountRequests = 1L });
        else
            currentAnalytics.CountRequests += 1;

        Db.SaveChanges();
    }

    public List<ModelAnalytic> GetAnalytics(long chatId)
    {
        return Db.ModelsAnalytics
            .Where(a => a.ChatId == chatId)
            .OrderByDescending(a => a.CountRequests)
            .ToList();
    }
}