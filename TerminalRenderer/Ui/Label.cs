using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

internal sealed class Label : Control
{
    public string Text { get; set; }
    public AnsiColor Foreground { get; set; } = AnsiColor.Yellow;
    public AnsiColor Background { get; set; } = AnsiColor.DarkBlue;

    public Label(string text) => Text = text;

    public override Size Measure(Size available) =>
        new(Math.Min(Text.Length, available.Width), 1);

    public override void Render(FrameBuffer fb)
    {
        Draw.Text(fb, Bounds.X, Bounds.Y, Text, Foreground, Background);
        base.Render(fb);
    }

}
