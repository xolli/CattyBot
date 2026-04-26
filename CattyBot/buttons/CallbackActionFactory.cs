using CattyBot.database;
using CattyBot.services;

namespace CattyBot.buttons;

public class CallbackActionFactory
{
    private readonly Dictionary<string, ICallbackAction> _callbackActions;

    private readonly ICallbackAction _sysPromptEditAction;

    private readonly ICallbackAction _sysPromptDeleteAction;

    private readonly ICallbackAction _setSystemPromptAction;

    public CallbackActionFactory(ResponseConfigService responseConfigService,
        SystemPromptService systemPromptService)
    {
        _callbackActions = new Dictionary<string, ICallbackAction>
        {
            // { "englishLanguage", new SetEnglish(localeService) },
            // { "russianLanguage", new SetRussian(localeService) },
            { "removeMessage", new RemoveMessage() }
        };
        _sysPromptEditAction = new SysPromptEditAction();
        _sysPromptDeleteAction = new SysPromptDeleteAction(systemPromptService);
        _setSystemPromptAction = new SetSystemPrompt(responseConfigService);
    }

    public ICallbackAction? GetCallbackActionByName(string name)
    {
        if (name.StartsWith("sysPromptEdit:")) return _sysPromptEditAction;
        if (name.StartsWith("sysPromptDelete:")) return _sysPromptDeleteAction;
        if (name.StartsWith("setSysPrompt:")) return _setSystemPromptAction;
        return _callbackActions.GetValueOrDefault(name);
    }
}