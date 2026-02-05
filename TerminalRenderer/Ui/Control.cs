
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

internal abstract class Control
{
    public List<Control> Children { get; } = [];

    public string? Id { get; set; }
    public string? ClassName { get; set; }
    public bool IsVisible { get; set; } = true;

    public Rect Bounds { get; set; }
    public bool Focusable { get; protected set; }


    public Control Add(Control child)
    {
        Children.Add(child); 
        return this;
    }

    public virtual Size Measure(Size available)
    {
        foreach (Control child in Children)
            child.Measure(available);
        return new Size(0, 0);
    }

    public virtual void Arrange(Rect rect)
    {
        Bounds = rect;
        foreach (Control child in Children)
            child.Arrange(rect);
    }

    public virtual void Render(FrameBuffer fb)
    {
        if (!IsVisible) return;
        foreach (var c in Children)
            c.Render(fb);
    }

    public virtual void OnKey(ConsoleKeyInfo key) { }
}

