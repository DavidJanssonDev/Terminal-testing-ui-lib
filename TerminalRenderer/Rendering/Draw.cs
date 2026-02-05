using System.Runtime.ExceptionServices;

namespace TerminalRendererProject.Rendering;

internal static class Draw
{
    public static void Text(FrameBuffer fb, int x, int y, string text, AnsiColor fg, AnsiColor bg)
    {
        for (int i = 0; i < text.Length; i++)
            fb.Set(x + i, y, new Cell(text[i], fg, bg));
    }

    public static void Fill(FrameBuffer fb, int x, int y, int w, int h, char ch, AnsiColor fg, AnsiColor bg)
    {
        for (int yy = 0; yy < h; yy++)
            for (int xx = 0; xx < w; xx++)
                fb.Set(x + xx, y + yy, new Cell(ch, fg, bg));
    }

    public static void Box(FrameBuffer fb, int x, int y, int w, int h, AnsiColor fg, AnsiColor bg)
    {
        for (int i = 0; i < w; i++)
        {
            fb.Set(x + i, y, new Cell('-', fg, bg));
            fb.Set(x + i, y + h - 1, new Cell('-', fg, bg));
        }

        for (int i = 0; i < h; i++)
        {
            fb.Set(x, y + i, new Cell('|', fg, bg));
            fb.Set(x + w - 1, y + i, new Cell('|', fg, bg));
        }

        fb.Set(x, y, new Cell('+', fg, bg));
        fb.Set(x + w - 1, y, new Cell('+', fg, bg));
        fb.Set(x, y + h - 1, new Cell('+', fg, bg));
        fb.Set(x + w - 1, y + h - 1, new Cell('+', fg, bg));
    }
}
