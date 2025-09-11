using System;
using System.Collections.Generic;
using ECommons.DalamudServices;
using Ocelot.Services;
using Ocelot.Services.Translation;
using Ocelot.Services.Windows;

namespace Ocelot.Commands;

public class MainOcelotCommand : OcelotCommand
{
    private static ITranslationService Translator {
        get => OcelotServices.GetCached<ITranslationService>();
    }

    private static IWindowManager WindowManager {
        get => OcelotServices.GetCached<IWindowManager>();
    }

    public override string Command { get; init; } = $"/{Svc.PluginInterface.InternalName.ToLower()}";

    public override string Description { get; init; } = "";

    public bool IncludeConfigHandler { get; init; } = true;

    public bool IncludeLanguageHandler { get; init; } = true;

    private List<(Func<List<string>, bool> ShouldRun, Action<List<string>> Execute)> handlers;

    public MainOcelotCommand()
    {
        handlers = [
            ( // Config
                ShouldRun: arguments => IncludeConfigHandler && arguments is ["config"] or ["cfg"] or ["c"],
                Execute: _ => { WindowManager.ToggleConfigUI(); }
            ),
            ( // Language
                ShouldRun: arguments => IncludeLanguageHandler && arguments is ["language", _],
                Execute: arguments => {
                    var lang = arguments[1];
                    if (!Translator.HasLanguage(lang))
                    {
                        var supported = string.Join(", ", Translator.Languages);
                        Svc.Chat.PrintError($"{lang} is not supported. ({supported})");
                        return;
                    }

                    Translator.SetLanguage(lang);
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


        WindowManager.ToggleMainUI();
    }
}
