using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

internal sealed class Button : Control
{
    public string Text { get; set; }
    public bool IsFocused { get; set; }
    public Action? OnClick { get; set; }

    public Button(string text)
    {
        Text = text;
        Focusable = true;
    }

    public override Size Measure(Size available) =>
        new(Math.Min(Text.Length + 6, available.Width), 3);

    public override void Render(FrameBuffer fb)
    {
        var bg = IsFocused ? AnsiColor.Gray : AnsiColor.DarkGray;
        var fg = IsFocused ? AnsiColor.Black : AnsiColor.White;

        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', fg, bg);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.White, bg);

        Draw.Text(
            fb,
            Bounds.X + (Bounds.Width - Text.Length) / 2,
            Bounds.Y + Bounds.Height / 2,
            Text,
            fg,
            bg
        );

        base.Render(fb);
    }

    public override void OnKey(ConsoleKeyInfo key)
    {
        if (key.Key is ConsoleKey.Enter or ConsoleKey.Spacebar)
            OnClick?.Invoke();
    }
}
