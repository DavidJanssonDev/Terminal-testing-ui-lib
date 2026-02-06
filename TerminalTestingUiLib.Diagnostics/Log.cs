using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalTestingUiLib.Diagnostics;

public static class Log
{
    // Sett this to filter logs in production if you want later.
    public static LogLevel MinmumLevel { get; set; } = LogLevel.Debug;

    public static void Debug(string category, string message, object? payload = null)
        => Write(LogLevel.Debug, category, message, payload);
    public static void Info(string category, string message, object? payload = null)
        => Write(LogLevel.Info, category, message, payload);
    public static void Warn(string category, string message, object? payload = null)
        => Write(LogLevel.Warn, category, message, payload);
    public static void Error(string category, string message, object? payload = null)
        => Write(LogLevel.Error, category, message, payload);

    private static void Write(LogLevel level, string category, string message, object? payload)
    {
        if (level < MinmumLevel)
        {
            return;
        }

        // Important : this touches DiagnosticsHost.Instance, which ensure the server starts.
        DiagnosticsHost.Instance.Publish(new DiagnosticsEvent(
            Type: "log",
            Utc: DateTime.UtcNow,
            Level: level,
            Category: category,
            Message: message,
            Payload: payload
        ));
    }
}
