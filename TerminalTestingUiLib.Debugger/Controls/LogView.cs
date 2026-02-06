using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class LogView : Control
{
    private readonly DebuggerModel _model;

    public int ScrollOffset { get; private set; } // 0 = bottom (latest)

    public LogView(DebuggerModel model)
    {
        _model = model;
        Focusable = false;
    }

    public void ScrollLines(int delta)
    {
        // Positive delta = scroll up (older)
        ScrollOffset = Math.Max(0, ScrollOffset + delta);
    }

    public void ScrollPages(int pages)
    {
        int page = Math.Max(1, Bounds.Height - 2);
        ScrollLines(pages * page);
    }

    public void ScrollToBottom() => ScrollOffset = 0;

    public override Size Measure(Size available) => available;

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, AnsiColor.Black);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.DarkGray, AnsiColor.Black);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y, " Logs ", AnsiColor.White, AnsiColor.Black);

        int innerX = Bounds.X + 1;
        int innerY = Bounds.Y + 1;
        int innerW = Math.Max(0, Bounds.Width - 2);
        int innerH = Math.Max(0, Bounds.Height - 2);

        var logs = _model.Logs;
        int count = logs.Count;

        // Show last lines, offset upward
        int startIndex = Math.Max(0, count - innerH - ScrollOffset);
        int endIndex = Math.Min(count, startIndex + innerH);

        int row = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            string line = logs[i];
            string clipped = Clip(line, innerW);
            Draw.Text(fb, innerX, innerY + row, clipped, AnsiColor.Gray, AnsiColor.Black);
            row++;
        }

        base.Render(fb);
    }

    private static string Clip(string s, int max)
    {
        if (max <= 0) return "";
        return s.Length <= max ? s : s[..max];
    }
}
