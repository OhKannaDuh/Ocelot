namespace Ocelot.Modules;

public interface IModule
{
    ModuleConfig? Config { get; }

    void Debug(string log);

    void Error(string log);

    void Fatal(string log);

    void Info(string log);

    void Verbose(string log);

    void Warning(string log);

    string Translate(string key);

    string Trans(string key);

    string T(string key);
}
