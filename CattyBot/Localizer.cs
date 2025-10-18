using System.Globalization;
using CattyBot.database;
using CattyBot.Resources;

namespace CattyBot;

public static class Localizer
{
    public static string GetValue(string key, Locale language)
    {
        var ruValue = LanguageResources.ResourceManager.GetString(key, new CultureInfo("ru"));
        return language switch
        {
            Locale.EN => LanguageResources.ResourceManager.GetString(key) ?? key,
            Locale.RU => ruValue ?? LanguageResources.ResourceManager.GetString(key) ?? key,
            _ => LanguageResources.ResourceManager.GetString(key) ?? key
        };
    }
}