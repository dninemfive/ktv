using d9.ktv.ActivityLogger;
using d9.utl;
using d9.utl.compat;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace d9.ktv;

public class Program
{
    public static class Args
    {
        public static readonly string ConfigPath = CommandLineArgs.TryGet(nameof(ConfigPath).toCamelCase(), CommandLineArgs.Parsers.FilePath)
                               ?? "config.json";
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
        using KtvService service = new(config, log);
        await service.Run();
    }
}