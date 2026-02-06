using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class StatusBar : Control
{
    public string LeftText { get; set; } = "";
    public string RightText { get; set; } = "";

    public override Size Measure(Size available) => new(available.Width, 1);

    public override void Arrange(Rect rect)
    {
        // Force height 1
        base.Arrange(new Rect(rect.X, rect.Y, rect.Width, 1));
    }

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, 1, ' ', AnsiColor.Black, AnsiColor.Gray);

        Draw.Text(fb, Bounds.X + 1, Bounds.Y, Truncate(LeftText, Bounds.Width - 2), AnsiColor.Black, AnsiColor.Gray);

        string right = Truncate(RightText, Bounds.Width - 2);
        int rx = Bounds.X + Bounds.Width - 1 - right.Length;
        if (rx > Bounds.X)
        {
            Draw.Text(fb, rx, Bounds.Y, right, AnsiColor.Black, AnsiColor.Gray);
        }

        base.Render(fb);
    }

    private static string Truncate(string s, int max)
    {
        if (max <= 0) return "";
        return s.Length <= max ? s : s[..max];
    }
}
