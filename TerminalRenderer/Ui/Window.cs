using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

public sealed class Window : Control
{
    public string Title { get; }

    public Window(string title) => Title = title;

    public override Size Measure(Size available) => available;

    public override void Arrange(Rect rect)
    {
        Bounds = rect;

        var inner = new Rect(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
        foreach (var c in Children)
            c.Arrange(inner);
    }

    public override void Render(FrameBuffer fb)
    {
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, AnsiColor.DarkBlue);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.White, AnsiColor.DarkBlue);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y + 1, Title, AnsiColor.White, AnsiColor.DarkCyan);

        base.Render(fb);
    }
}
