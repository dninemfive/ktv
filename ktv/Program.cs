using d9.utl;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly string ConfigPath = CommandLineArgs.TryGet(nameof(ConfigPath).toCamelCase(), CommandLineArgs.Parsers.FilePath)
                               ?? "config.json".AbsoluteOrInBaseFolder();
        public static readonly bool PrintToConsole = CommandLineArgs.GetFlag(nameof(PrintToConsole).toCamelCase());
    }
    public static async Task Main()
    {
        // not `using` because the service will dispose this for us
        // todo: redo Log class with ability to use async writing.
        // possibly just Log(params Func<object?>[] callbacks)?
        // with static methods which act as sugar for common log stuff like writing to a file
        // possibly also have like an `event` on it idk
        Log log = new(DateTime.Now.GenerateLogFile(), mode: Log.Mode.WriteImmediate);
        KtvConfig config;
        try
        {
            config = Config.Load<KtvConfig>(Args.ConfigPath);
        } 
        catch(Exception e)
        {
            log.WriteLine($"Could not find valid config at expected path {Path.GetFullPath(Args.ConfigPath)}!\n{e.GetType().Name}: {e.Message}");
            return;
        }
        // todo: somehow multiple progress reports within the same method can cause a crash here
        // probably needs a custom method which handles this, possibly aggregating reports into an async-safe collection and writing sequentially
        Progress<string> progress = new(log.WriteLine);
        KtvService service = KtvService.CreateAndLog(config, progress);
        await service.Run();
    }
}