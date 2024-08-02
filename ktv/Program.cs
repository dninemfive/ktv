﻿using d9.utl;

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
        Log log = Log.ConsoleAndFile(DateTime.Now.GenerateLogFile(), true);
        await log.WriteLine($"test");
        KtvConfig config;
        try
        {
            config = Config.Load<KtvConfig>(Args.ConfigPath);
        } 
        catch(Exception e)
        {
            await log.WriteLine($"Could not find valid config at expected path {Path.GetFullPath(Args.ConfigPath)}!\n{e.GetType().Name}: {e.Message}");
            return;
        }
        // await Task.Delay(1000000);
        KtvService service = KtvService.CreateAndLog(config, log);
        await service.Run();
    }
}