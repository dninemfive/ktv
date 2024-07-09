using d9.utl;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly string ConfigPath = CommandLineArgs.TryGet(nameof(ConfigPath).toCamelCase(), CommandLineArgs.Parsers.FilePath)
                               ?? "config.json".AbsolutePath();
        public static readonly bool PrintToConsole = CommandLineArgs.GetFlag(nameof(PrintToConsole).toCamelCase());
    }
    public static async Task Main()
    {
        // not `using` because the service will dispose this for us
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
        Progress<string> progress = new(log.WriteLine);
        KtvService service = new(config, progress);
        await service.Run();
    }
}