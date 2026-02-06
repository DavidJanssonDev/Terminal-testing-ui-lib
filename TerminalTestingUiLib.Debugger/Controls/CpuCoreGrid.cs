using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class CpuCoreGrid : Control
{
    private readonly DebuggerModel _model;

    public CpuCoreGrid(DebuggerModel model)
    {
        _model = model;
    }

    public override Size Measure(Size available) => available;

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, AnsiColor.Black);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.DarkGray, AnsiColor.Black);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y, " CPU Cores ", AnsiColor.White, AnsiColor.Black);

        double[] cores = _model.CpuCoresPct;
        if (cores.Length == 0)
        {
            Draw.Text(fb, Bounds.X + 2, Bounds.Y + 2, "Waiting for cpu.cores_pct...", AnsiColor.DarkGray, AnsiColor.Black);
            return;
        }

        int innerX = Bounds.X + 1;
        int innerY = Bounds.Y + 1;
        int innerW = Math.Max(0, Bounds.Width - 2);
        int innerH = Math.Max(0, Bounds.Height - 2);

        // Tile size
        const int tileW = 8;
        const int tileH = 4;

        int cols = Math.Max(1, innerW / tileW);
        int rows = Math.Max(1, innerH / tileH);

        int maxTiles = cols * rows;
        int tiles = Math.Min(maxTiles, cores.Length);

        for (int i = 0; i < tiles; i++)
        {
            int col = i % cols;
            int row = i / cols;

            int x = innerX + col * tileW;
            int y = innerY + row * tileH;

            double pct = cores[i];
            var bg = BgFor(pct);

            Draw.Fill(fb, x, y, tileW, tileH, ' ', AnsiColor.Black, bg);
            Draw.Box(fb, x, y, tileW, tileH, AnsiColor.Black, bg);

            string top = $"C{i}";
            string mid = $"{pct:0}%";

            Draw.Text(fb, x + 1, y + 1, top, AnsiColor.Black, bg);
            Draw.Text(fb, x + 1, y + 2, mid, AnsiColor.Black, bg);
        }

        base.Render(fb);
    }

    private static AnsiColor BgFor(double pct)
    {
        if (pct < 10) return AnsiColor.DarkBlue;
        if (pct < 25) return AnsiColor.DarkCyan;
        if (pct < 50) return AnsiColor.DarkGreen;
        if (pct < 75) return AnsiColor.DarkYellow;
        return AnsiColor.DarkRed;
    }
}
