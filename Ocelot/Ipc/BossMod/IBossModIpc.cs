namespace Ocelot.Ipc.BossMod;

public interface IBossModIpc
{
    void Create(string presetSerialized, bool overwrite = false);

    bool Activate(string name);

    bool Deactivate(string name);
}
