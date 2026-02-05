
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Ui;

internal abstract class Control
{
    public List<Control> Children { get; } = new();

    // The final position and size given by layout.
    public Rect Bounds { get; private set; }
    
    // For focus/input
    public bool Focusable { get; protected set; }

    // Called during layout: "How big do you want to be?"
    public virtual Size Measure(Size available)
    {
        // Deafult behavior: measure children but claim no size
        foreach (Control child in Children)
        {
            child.Measure(available);
        }

        return Size.Zero;
    }

    // Called during layout: "Here is tour final rectangle."
    public virtual void Arrange(Rect finalRect)
    {
        Bounds = finalRect;

        // Deafult behavior: give all children the same space.
        foreach(Control child in Children)
        {
            child.Arrange(finalRect);
        }
    }

    // Called during render: paint into the framebuffer.
    public virtual void Render(FrameBuffer fb)
    {
        foreach (Control child in Children)
        {
            child.Render(fb); 
        }
    }

    // Input hooks (minimal for now)
    public virtual void OnKey(ConsoleKeyInfo key) { }
}

