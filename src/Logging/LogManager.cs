using System;
using System.IO;
using System.Linq;
using Lotus.API.Odyssey;
using Lotus.API.Reactive;
using VentLib.Logging.Appenders;
using VentLib.Utilities;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Harmony.Attributes;

namespace Lotus.Logging;

[LoadStatic]
public class LogManager
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LogManager));
    private static LogUI _logUI = null!;

    private static Remote<FlushingMemoryAppender> _sessionAppender;
    private static DirectoryInfo _dailyDirectory = null!;
    private static int _logIndex;


    static LogManager()
    {
        string directory = CreateSessionDirectory();
        _sessionAppender = GlobalLogAppenders.AddAppender(new FlushingMemoryAppender(directory, null!, LogLevel.Info) { AutoFlush = false }).Cast<FlushingMemoryAppender>();
        Hooks.NetworkHooks.GameJoinHook.Bind(nameof(LogManager), e => BeginGameLogSession(e.IsNewLobby));
    }

    [QuickPostfix(typeof(HudManager), nameof(HudManager.Start))]
    public static void AttachUI(HudManager __instance)
    {
        _logUI = __instance.gameObject.AddComponent<LogUI>();
        _logUI.PassRequirements(__instance);
        _logUI.OnTextSubmit += logName => Async.ExecuteThreaded(() => WriteSessionLog(logName));
    }

    public static void OpenLogUI()
    {
        _logUI.Open();
    }

    public static void SendInGame(string message, params object[] args)
    {
        StaticLogger.Debug($"Sending In Game: {message}", args);
        if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(message);
    }
    public static void BeginGameLogSession(bool isNewGame)
    {
        string directory = CreateSessionDirectory();
        if (!isNewGame)
        {
            var appender = _sessionAppender.Get();
            appender.FileNamePattern = $"_game-{LogDirectory.GetLogs("_game-", _dailyDirectory).Count().ToString()}.txt";
            appender.LogFile = LogDirectory.CreateLog(appender.FileNamePattern, _dailyDirectory);
            appender.Clear();
        }

        _sessionAppender.Delete();
        _sessionAppender = GlobalLogAppenders.AddAppender(new FlushingMemoryAppender(directory, null!, LogLevel.Info) { AutoFlush = false }).Cast<FlushingMemoryAppender>();
        _logIndex = 0;
    }

    public static int GetLineCount(FileInfo file)
    {
        if (file == null)
        {
            StaticLogger.Warn("Nil fileinfo in LogManager.GetLineCount");
            return 0;
        }
        using (StreamReader reader = file.OpenText())
        {
            int lineCount = 0;

            // Read each line until the end of the file
            while (reader.ReadLine() != null)
            {
                lineCount++;
            }
            return lineCount;
        }
    }

    public static void WriteSessionLog(string logName)
    {
        if (logName == "")
        {
            logName = DateTime.Now.ToString("yyyy-MM-dd") + "-session-";
            logName += LogDirectory.GetLogs(logName, _dailyDirectory).Count().ToString();
        }
        if (!logName.Contains('.')) logName += ".txt";

        var appender = _sessionAppender.Get();

        log.High($"Dumping session logs as pattern: \"{logName}\".");
        appender.FileNamePattern = logName;
        FileInfo file = appender.CreateNewFile();
        int logCount = GetLineCount(appender.LogFile);
        appender.Clear();
        _logIndex += logCount;

        LogManager.SendInGame($"Successfully saved {logCount} logs from current session. (Filename={file.Name})", LogLevel.High);
    }

    private static string CreateSessionDirectory()
    {
        string dateString = DateTime.Now.ToString("yyyy-MM-dd");
        string dir = "logs/sessions/" + dateString;
        _dailyDirectory = new DirectoryInfo(dir);
        if (!_dailyDirectory.Exists) _dailyDirectory.Create();
        return dir;
    }
}