using System;
using System.Collections.Generic;
using ECommons.DalamudServices;

namespace Ocelot.Commands;

public class MainOcelotCommand : OcelotCommand
{
    public override string Command { get; init; } = $"/{Svc.PluginInterface.InternalName.ToLower()}";

    public override string Description { get; init; } = "";

    public bool IncludeConfigHandler { get; init; } = true;

    public bool IncludeLanguageHandler { get; init; } = true;

    private List<(Func<List<string>, bool> ShouldRun, Action<List<string>> Execute)> handlers;

    public MainOcelotCommand()
    {
        handlers =
        [
            ( // Config
                ShouldRun: arguments => IncludeConfigHandler && arguments is ["config"] or ["cfg"] or ["c"],
                Execute: _ => { OcelotPlugin.Plugin.Windows.ToggleConfigUI(); }
            ),
            ( // Language
                ShouldRun: arguments => IncludeLanguageHandler && arguments is ["language", _],
                Execute: arguments =>
                {
                    var lang = arguments[1];
                    if (!I18N.HasLanguage(lang))
                    {
                        var supported = string.Join(", ", I18N.GetAllLanguageKeys());
                        Svc.Chat.PrintError($"{lang} is not supported. ({supported})");
                        return;
                    }

                    I18N.SetLanguage(lang);
                    OcelotPlugin.Plugin.OcelotConfig.OcelotCoreConfig.Language = lang;
                    OcelotPlugin.Plugin.OcelotConfig.Save();
                }
            ),
        ];
    }

    public void AddHandler(Func<List<string>, bool> shouldRun, Action<List<string>> execute)
    {
        handlers.Add((shouldRun, execute));
    }

    public override void Execute(List<string> arguments)
    {
        foreach (var handler in handlers)
        {
            if (handler.ShouldRun(arguments))
            {
                handler.Execute(arguments);
                return;
            }
        }

        OcelotPlugin.Plugin.Windows.ToggleMainUI();
    }
}
