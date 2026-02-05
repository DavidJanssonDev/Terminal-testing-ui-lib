using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.Ui;

internal sealed class StackPanel : Control
{
    public int Spacing { get; set; } = 1;


    public override Size Measure(Size available)
    {
        int totalHeight = 0;
        int maxWidth = 0;

        foreach (Control child in Children)
        {
            Size childSize = child.Measure(available);
            maxWidth = Math.Max(maxWidth, childSize.Width);
            totalHeight += childSize.Height;
        }

        if (Children.Count > 1)
        {
            totalHeight += Spacing * (Children.Count - 1);
        }

        maxWidth = Math.Min(maxWidth, available.Width);
        totalHeight = Math.Min(totalHeight, available.Height);

        return new Size(maxWidth, totalHeight);
    }

    public override void Arrange(Rect finalRect)
    {
        base.Arrange(finalRect);

        int y = finalRect.Y;

        foreach (Control child in Children)
        {
            Size desired = child.Measure(new Size(finalRect.Width, finalRect.Height));

            // Give child full width, its desired height.
            int h = Math.Min(desired.Height, finalRect.Bottom - y);
            child.Arrange(new Rect(finalRect.X, y, finalRect.Width, h));

            y += h + Spacing;
            if (y >= finalRect.Bottom)
            {
                break;
            }
        }
    }
}
