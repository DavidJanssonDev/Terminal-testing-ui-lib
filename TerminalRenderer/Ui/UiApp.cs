using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.UI;

internal sealed class UiApp
{
    private readonly FrameBuffer _fb;
    private readonly List<Button> _focusables = new();
    private int _focusIndex;

    public Control Root { get; }

    public UiApp(Control root, FrameBuffer fb)
    {
        Root = root;
        _fb = fb;
        CollectFocusables(root);
        UpdateFocus();
    }
    public void Layout(int w, int h)
    {
        Root.Measure(new Size(w, h));
        Root.Arrange(new Rect(0, 0, w, h));
    }

    public void Render()
    {
        _fb.Clear(new Cell(' ', AnsiColor.Gray, AnsiColor.Black));
        Root.Render(_fb);
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Tab)
        {
            _focusIndex = (_focusIndex + 1) % _focusables.Count;
            UpdateFocus();
        }
        else if (_focusables.Count > 0)
        {
            _focusables[_focusIndex].OnKey(key);
        }
    }

    private void CollectFocusables(Control c)
    {
        if (c is Button b) _focusables.Add(b);
        foreach (var child in c.Children)
            CollectFocusables(child);
    }

    private void UpdateFocus()
    {
        for (int i = 0; i < _focusables.Count; i++)
            _focusables[i].IsFocused = i == _focusIndex;
    }
}
