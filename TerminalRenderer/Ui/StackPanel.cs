using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.UI;

public sealed class StackPanel : Control
{
    public int Spacing { get; set; } = 1;

    public override Size Measure(Size available)
    {
        int h = 0;
        int w = 0;

        foreach (var c in Children)
        {
            var s = c.Measure(available);
            h += s.Height + Spacing;
            w = Math.Max(w, s.Width);
        }

        return new Size(w, h);
    }

    public override void Arrange(Rect rect)
    {
        
        Bounds = rect;
        int y = rect.Y;

        foreach (var c in Children)
        {
            var s = c.Measure(new Size(rect.Width, rect.Height));
            c.Arrange(new Rect(rect.X, y, rect.Width, s.Height));
            y += s.Height + Spacing;
        }

    }
}
