namespace TerminalTestingUiLib.Diagnostics;

public sealed record DiagnosticsEvent(
    string Type,          // "log", later: "metric", "snapshot"
    DateTime Utc,
    LogLevel? Level,
    string Category,
    string Message,
    object? Payload
);
