using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

public sealed class UiApp
{
    private readonly FrameBuffer _fb;
    private readonly List<Button> _focusables = new();
    private int _focusIndex;

    private bool _isVisualDirty = true;
    private bool _isLayoutDirty = true;

    private int _lastWidth;
    private int _lastHeight;


    public Control Root { get; }

    public UiApp(Control root, FrameBuffer fb)
    {
        Root = root;
        _fb = fb;

        CollectFocusables(root);
        UpdateFocus();

        // Default layout size is unknown until Layout() is called.
        _lastWidth = fb.Width;
        _lastHeight = fb.Height;
    }

    public void InvalidateVisual() => _isVisualDirty = true;
    public void InvalidateLayout()
    {
        _isVisualDirty = true;
        _isLayoutDirty = true;
    }

    public void Layout(int w, int h)
    {
        _lastWidth = w;
        _lastHeight = h;

        Root.Measure(new Size(w, h));
        Root.Arrange(new Rect(0, 0, w, h));

        _isLayoutDirty = false;
        _isVisualDirty = true; // layout pass means visual needs repaint
    }
    public void Render()
    {
        _fb.Clear(new Cell(' ', AnsiColor.Gray, AnsiColor.Black));
        Root.Render(_fb);
    }

    /// <summary>
    /// Processes pending layout/render work if dirty.
    /// Returns true if the framebuffer content changed and should be presented.
    /// </summary>
    public bool Tick()
    {
        if (_isLayoutDirty)
        {
            Layout(_lastWidth, _lastHeight);
            return true;
        }

        if (_isVisualDirty)
        {
            Render();
            _isVisualDirty = false;
            return true;
        }

        return false;
    }
    public void HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Tab)
        {
            if (_focusables.Count == 0)
            {
                return;
            }

            _focusIndex = (_focusIndex + 1) % _focusables.Count;
            UpdateFocus();
            InvalidateVisual();
            return;
        }

        if (_focusables.Count > 0)
        {
            _focusables[_focusIndex].OnKey(key);

            // Safe default for now: input might change visuals
            InvalidateVisual();
        }
    }




    private void CollectFocusables(Control c)
    {
        if (c is Button b)
        {
            _focusables.Add(b);
        }

        foreach (var child in c.Children)
        {
            CollectFocusables(child);
        }
    }
    private void UpdateFocus()
    {
        for (int i = 0; i < _focusables.Count; i++)
        {
            _focusables[i].IsFocused = i == _focusIndex;
        }
    }

}
