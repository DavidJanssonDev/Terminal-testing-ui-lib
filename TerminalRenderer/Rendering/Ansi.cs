using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.Rendering;

internal static class Ansi
{
    public static string CursorHome  => "\x1b[H";
    public static string ClearScreen => "\x1b[2J";
    public static string HideCursor => "\x1b[?25l";
    public static string ShowCursor => "\x1b[?25H";
    public static string Reset => "\x1b[0m";
    
    // ANSI cursor positions are 1-based (1..N), not 0-based.
    public static void AppendMoveCursor(StringBuilder sb, int x, int y)
    {
        int row = y + 1;
        int col = x + 1;
        sb.Append("\x1b[");
        sb.Append(row);
        sb.Append(';');
        sb.Append(col);
        sb.Append('H');
    }
}
