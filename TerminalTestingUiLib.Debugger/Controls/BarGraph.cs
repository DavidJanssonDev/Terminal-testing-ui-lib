using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class BarGraph : Control
{
    public string Title { get; set; } = "Graph";
    public Func<IReadOnlyList<double>>? GetSeries { get; set; }

    public AnsiColor Foreground { get; set; } = AnsiColor.White;
    public AnsiColor Background { get; set; } = AnsiColor.Black;

    public override Size Measure(Size available) => available;

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, Background);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.DarkGray, Background);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y, $" {Title} ", Foreground, Background);

        if (GetSeries is null)
        {
            return;
        }

        IReadOnlyList<double> series = GetSeries();
        if (series.Count == 0)
        {
            Draw.Text(fb, Bounds.X + 2, Bounds.Y + 2, "Waiting for samples...", AnsiColor.DarkGray, Background);
            return;
        }

        int innerX = Bounds.X + 1;
        int innerY = Bounds.Y + 1;
        int innerW = Math.Max(0, Bounds.Width - 2);
        int innerH = Math.Max(0, Bounds.Height - 2);

        if (innerW <= 0 || innerH <= 0)
        {
            return;
        }

        // We draw the last innerW samples (one column per sample)
        int take = Math.Min(innerW, series.Count);
        int start = series.Count - take;

        double max = 1;
        for (int i = start; i < series.Count; i++)
        {
            max = Math.Max(max, series[i]);
        }

        for (int x = 0; x < take; x++)
        {
            double v = series[start + x];
            int barH = (int)Math.Round((v / max) * innerH);
            barH = Math.Clamp(barH, 0, innerH);

            for (int y = 0; y < innerH; y++)
            {
                int drawY = innerY + (innerH - 1 - y);
                char ch = y < barH ? '█' : ' ';
                fb.Set(innerX + x, drawY, new Cell(ch, Foreground, Background));
            }
        }

        base.Render(fb);
    }
}
