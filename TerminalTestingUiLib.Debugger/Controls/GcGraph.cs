using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class GcGraph : Control
{
    private readonly DebuggerModel _model;

    public GcGraph(DebuggerModel model)
    {
        _model = model;
    }

    public override Size Measure(Size available) => available;

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, AnsiColor.Black);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.DarkGray, AnsiColor.Black);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y, " GC (deltas) ", AnsiColor.White, AnsiColor.Black);

        int innerX = Bounds.X + 1;
        int innerY = Bounds.Y + 1;
        int innerW = Math.Max(0, Bounds.Width - 2);
        int innerH = Math.Max(0, Bounds.Height - 2);

        if (innerW <= 0 || innerH <= 0)
        {
            return;
        }

        // We'll show 3 lines of text + tiny bars (cheap and clear)
        string line1 = $"ΔGen0: {_model.GcDelta0}   Total: {_model.Gc0}";
        string line2 = $"ΔGen1: {_model.GcDelta1}   Total: {_model.Gc1}";
        string line3 = $"ΔGen2: {_model.GcDelta2}   Total: {_model.Gc2}";

        Draw.Text(fb, innerX, innerY + 0, Clip(line1, innerW), AnsiColor.White, AnsiColor.Black);
        if (innerH > 1) Draw.Text(fb, innerX, innerY + 1, Clip(line2, innerW), AnsiColor.White, AnsiColor.Black);
        if (innerH > 2) Draw.Text(fb, innerX, innerY + 2, Clip(line3, innerW), AnsiColor.White, AnsiColor.Black);

        base.Render(fb);
    }

    private static string Clip(string s, int max) => max <= 0 ? "" : (s.Length <= max ? s : s[..max]);
}
