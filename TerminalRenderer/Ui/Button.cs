using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Ui;

internal sealed class Button : Control
{
    public string Text { get; set; }
    public bool IsFocused { get; set; }

    public event Action? Clicked;

    public Button(string text)
    {
        Text = text;
        Focusable = true;
    }

    public override Size Measure(Size available)
    {
        // Terminal "button" is a smal bot with padding
        int width = Math.Min(available.Width, Text.Length + 6);
        int height = Math.Min(available.Height, 3);
        return new Size(width, height);
    }

    public override void Render(FrameBuffer fb)
    {
        AnsiColor bg = IsFocused ? AnsiColor.Gray : AnsiColor.DarkGray;
        AnsiColor fg = IsFocused ? AnsiColor.Black : AnsiColor.White;

        Draw.Fill(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, ' ', fg, bg);
        Draw.Box(fb, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, AnsiColor.White, bg);

        string content = Text;
        int TextX = Bounds.X + (Bounds.Width - content.Length) / 2;
        int TextY = Bounds.Y + (Bounds.Height / 2);

        Draw.Text(fb, TextX, TextY, content, fg, bg);
        
        base.Render(fb);
    }

    public override void OnKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
        {
            Clicked?.Invoke();
        }
    }
}
