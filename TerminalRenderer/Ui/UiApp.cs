using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Ui;

internal sealed class UiApp
{
    private readonly FrameBuffer m_FrameBuffer;

    public Control Root { get; }

    private readonly List<Control> m_Focusables = new();
    private int m_FocusIndex = 0;

    public UiApp(Control root, FrameBuffer fb)
    {
        Root = root;
        m_FrameBuffer = fb;

        RebuildFocusableList();
        ApplyFocus();
    }

    public void Layout(int width, int height)
    {
        Root.Measure(new Size(width, height));
        Root.Arrange(new Rect(0, 0, width, height));
    }

    public void Render()
    {
        m_FrameBuffer.Clear(new Cell(' ', AnsiColor.Gray, AnsiColor.Black));
        Root.Render(m_FrameBuffer);
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Tab)
        {
            MoveFocus(shift: (key.Modifiers & ConsoleModifiers.Shift) != 0);
            return;
        }

        if (m_Focusables.Count > 0)
        {
            m_Focusables[m_FocusIndex].OnKey(key);
        }
    }

    private void MoveFocus(bool shift)
    {
        if (m_Focusables.Count == 0)
        {
            return;
        }

        // Clear old focus
        SetFocusState(m_Focusables[m_FocusIndex], isFocused: false);

        m_FocusIndex = shift
            ? (m_FocusIndex - 1 + m_Focusables.Count) % m_Focusables.Count
            : (m_FocusIndex + 1) % m_Focusables.Count;

        ApplyFocus();
    }

    private void ApplyFocus()
    {
        if (m_Focusables.Count == 0)
        {
            return;
        }

        SetFocusState(m_Focusables[m_FocusIndex], isFocused: true);
    }
    private void RebuildFocusableList()
    {
        m_Focusables.Clear();
        CollectFocusables(Root, m_Focusables);
        m_FocusIndex = Math.Clamp(m_FocusIndex, 0, Math.Max(0, m_Focusables.Count - 1));
    }

    private static void CollectFocusables(Control rootControl, List<Control> list)
    {
        if (rootControl.Focusable)
        {
            list.Add(rootControl);
        }

        foreach (Control child in rootControl.Children)
        {
            CollectFocusables(child, list);
        }
    }

    private static void SetFocusState(Control control, bool isFocused)
    {
        // For now, only Button supports focus visuals.
        if (control is Button button)
        {
            button.IsFocused = isFocused;
        }
    }
}
