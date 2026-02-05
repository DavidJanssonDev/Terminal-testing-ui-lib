using System.Runtime.ExceptionServices;

namespace TerminalRendererProject.Rendering;

internal static class Draw
{
    public static void Text(FrameBuffer fb, int xCord, int yCord, string text, AnsiColor fg, AnsiColor bg)
    {
        for (int i = 0; i < text.Length; i++)
        {
            fb.Set(xCord + i, yCord, new Cell(text[i], fg, bg));
        }
    }

    public static void Box(FrameBuffer fb, int xCord, int yCord, int width, int height, AnsiColor fg, AnsiColor bg)
    {
        // Simple ASCII box. Later we can Support rounded corners, double lines, etc.
        char tl = '+';
        char tr = '+';
        char bl = '+';
        char br = '+';
        char hz = '-';
        char vt = '|';

        // Top/bottom edges
        for (int x = 0; x < width; x++)
        {
            fb.Set(xCord + x, yCord, new Cell(hz, fg, bg));
            fb.Set(xCord + x, yCord + height - 1, new Cell(hz, fg, bg));
        }

        // Left/right edges 
        for (int y = 0; y < height; y++)
        {
            fb.Set(xCord, yCord + y, new Cell(vt, fg, bg));
            fb.Set(xCord + width - 1, yCord + y, new Cell(vt, fg, bg));
        }

        // Corners
        fb.Set(xCord, yCord, new Cell(tl, fg, bg));
        fb.Set(xCord + width - 1, yCord, new Cell(tr, fg, bg));
        fb.Set(xCord, yCord + height - 1, new Cell(bl, fg, bg));
        fb.Set(xCord + width - 1, yCord + height - 1, new Cell(br, fg, bg));
    }

    public static void Fill(FrameBuffer fb, int xCord, int yCord, int width, int height, char ch, AnsiColor fg, AnsiColor bg)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                fb.Set(xCord + x, yCord + y, new Cell(ch, fg, bg));
            }
        }
    }
}
