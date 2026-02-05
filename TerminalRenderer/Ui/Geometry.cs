using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.Ui;

internal readonly record struct Size(int Width, int Height) 
{
    public static readonly Size Zero = new(0, 0);
}


internal readonly record struct Rect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;
    public int Bottom => Y + Height;
}
