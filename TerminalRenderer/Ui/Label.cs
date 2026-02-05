using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Ui;

internal sealed class Label : Control
{
    public string Text { get; set; }
    public AnsiColor Foreground { get; set; } = AnsiColor.Yellow;
    public AnsiColor Background { get; set; } = AnsiColor.DarkBlue;

    public Label(string text)
    {
        Text = text;
        Focusable = false;
    }

    public override Size Measure(Size available)
    {
        int width = Text.Length;
        int height = 1;
        
        // Clamp desired size to available (simple model)
        width = Math.Min(width, available.Width);
        height = Math.Min(height, available.Height);

        return new Size(width, height);
    }

    public override void Render(FrameBuffer fb)
    {
        Draw.Text(fb, Bounds.X, Bounds.Y, Text, Foreground, Background);
        base.Render(fb);
    }

}
