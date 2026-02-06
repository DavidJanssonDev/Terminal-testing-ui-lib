using System.Drawing;
using TerminalRenderer.UI;
using Size = TerminalRenderer.UI.Size;
namespace TerminalTestingUiLib.Debugger.Controls;

internal sealed class SplitPanel : Control
{
    public double LeftRatio { get; set; } = 0.55;

    public override Size Measure(Size available)
    {
        foreach (var c in Children)
        {
            c.Measure(available);
        }

        return available;
    }

    public override void Arrange(Rect rect)
    {
        base.Arrange(rect);

        if (Children.Count < 2)
        {
            return;
        }

        int leftW = (int)(rect.Width * LeftRatio);
        leftW = Math.Clamp(leftW, 10, rect.Width - 10);

        var left = new Rect(rect.X, rect.Y, leftW, rect.Height);
        var right = new Rect(rect.X + leftW, rect.Y, rect.Width - leftW, rect.Height);

        Children[0].Arrange(left);
        Children[1].Arrange(right);
    }
}
