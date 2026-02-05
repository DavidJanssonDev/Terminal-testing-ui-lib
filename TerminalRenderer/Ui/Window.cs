using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Ui;

internal sealed class Window : Control
{
    public string Title { get; set; }
    public int Padding { get; set; } = 1;
    public Window(string title)
    {
        Title = title;
        Focusable = false;
    }

    public override Size Measure(Size available)
    {
        // Window wants t o take all avaibole spance for now
        foreach (Control child in Children)
        {
            child.Measure(new Size(
                Math.Max(0, available.Width - 2 - Padding * 2),
                Math.Max(0, available.Height - 2 - Padding * 2)));
        }

        return available;
    }

    public override void Arrange(Rect finalRect)
    {
        base.Arrange(finalRect);

        // Inner area where children go (inside border + padding)
        Rect inner = new Rect(
            finalRect.X + 1 + Padding,
            finalRect.Y + 2 + Padding, // +2 because title bar row
            Math.Max(0, finalRect.Width - 2 - Padding * 2),
            Math.Max(0, finalRect.Height - 3 - Padding * 2));

        foreach (Control child in Children)
        {
            child.Arrange(inner);
        }
    }

    public override void Render(FrameBuffer fb)
    {
        // Backgrond and border
        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', AnsiColor.Gray, AnsiColor.DarkBlue);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.White, AnsiColor.DarkBlue);

        // Title bar
        Draw.Fill(fb, Bounds.X + 1, Bounds.Y + 1, Bounds.Width - 2, 1, ' ', AnsiColor.White, AnsiColor.DarkCyan);
        Draw.Text(fb, Bounds.X + 2, Bounds.Y + 1, Title, AnsiColor.White, AnsiColor.DarkCyan);
        base.Render(fb);
    }
}
