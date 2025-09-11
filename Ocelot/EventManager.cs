using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using Ocelot.Intents;
using Ocelot.Services;

namespace Ocelot;

public sealed class EventManager : IDisposable
{
    private bool initialized = false;


    private IReadOnlyList<IChatListener> chat = [];

    private IReadOnlyList<ITerritoryListener> territory = [];

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        Svc.Chat.ChatMessage += OnChatMessage;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    }

    internal void Refresh()
    {
        chat = OcelotServices.Container.GetAll<IChatListener>().ToList();
        territory = OcelotServices.Container.GetAll<ITerritoryListener>().ToList();
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        foreach (var c in chat)
        {
            if (c is IToggleable { IsEnabled: false })
            {
                continue;
            }

            c.OnChatMessage(type, timestamp, sender, message, isHandled);
        }
    }


    private void OnTerritoryChanged(ushort id)
    {
        foreach (var t in territory)
        {
            if (t is IToggleable { IsEnabled: false })
            {
                continue;
            }

            t.OnTerritoryChanged(id);
        }
    }


    public void Dispose()
    {
        Svc.Chat.ChatMessage -= OnChatMessage;
        Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;
    }
}
