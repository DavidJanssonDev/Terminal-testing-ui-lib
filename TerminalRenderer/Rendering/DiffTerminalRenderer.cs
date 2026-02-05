using System;
using System.Collections.Generic;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Rendering;

internal sealed class DiffTerminalRenderer
{
    private readonly int m_Width;
    private readonly int m_Height;

    private readonly StringBuilder m_StringBuilder;

    // Copy of what we last presented
    private readonly Cell[] m_PreviousBuffer;

    // Tracks the last emitted colors (so we don’t spam color codes)
    private bool m_HasColor;
    private AnsiColor m_LastFg;
    private AnsiColor m_LastBg;

    // Tracks wher the terminal cursor is ( so we don't spam cursor moves)
    private int m_CursorX;
    private int m_CursorY;
    private bool m_CursorKnow;


    public DiffTerminalRenderer(int width, int height)
    {
        m_Width = width;
        m_Height = height;

        // Step 1 used width * height + height for full-screen builds.
        // For diff rendering, we output much less most frames,
        // but we still want a reasonable starting capacity

        m_StringBuilder = new StringBuilder(width * height / 2);
        
        m_PreviousBuffer = new Cell[width * height];

        m_HasColor = false;
        m_LastFg = AnsiColor.Gray;
        m_LastBg = AnsiColor.Black;

        m_CursorKnow = false;
        m_CursorX = 0;
        m_CursorY = 0;

        // Initial terminal setup
        Console.Write(Ansi.ClearScreen);
        Console.Write(Ansi.CursorHome);
        Console.Write(Ansi.HideCursor);
    }

    public void Present(FrameBuffer current)
    {
        m_StringBuilder.Clear();

        ReadOnlySpan<Cell> currentBuffer = current.Cells;

        // First ever frame: we have no meaningful previous state, so paint all.
        // (We could detect this with a bool, but easiest is: if _cursorKnown is false and previous is default.)
        // Instead, we’ll just diff anyway; default previous cells are '\0' so they will differ.
        // To avoid printing weird chars, we treat '\0' as "definitely different".
        for (int y = 0;  y < m_Height; y++)
        {
            int rowStart = y * m_Width;

            for (int x = 0; x < m_Width; x++)
            {
                int idx = rowStart + x;

                Cell newCell = currentBuffer[idx];
                Cell oldCell = m_PreviousBuffer[idx];

                bool oldUninitialized = oldCell.Ch == '\0';

                if (!oldUninitialized && oldCell == newCell)
                {
                    continue; // Unchanged → skip
                }

                // Ensure Cursor is where we need it
                MoveCursorIfNeeded(x, y);

                // Ensure current colors
                AppendColorIfNeeded(newCell.Fg, newCell.Bg);

                // Outpout the character
                m_StringBuilder.Append(newCell.Ch);

                // Update cursor postion tracking
                m_CursorX = x + 1;
                m_CursorY = y;
                m_CursorKnow = true;

                // Update previous buffer cell
                m_PreviousBuffer[idx] = newCell;
            }

        }

        if (m_StringBuilder.Length > 0)
        {
            Console.Write(m_StringBuilder.ToString());
        }
    }

    public void Shutdown()
    {
        Console.Write(Ansi.Reset);
        Console.Write(Ansi.ShowCursor);
    }

    private void MoveCursorIfNeeded(int x, int y)
    {
        if (!m_CursorKnow || m_CursorX != x || m_CursorY != y)
        {
            Ansi.AppendMoveCursor(m_StringBuilder, x, y);
            m_CursorX = x;
            m_CursorY = y;
            m_CursorKnow = true;
        }   
    }

    private void AppendColorIfNeeded(AnsiColor fg, AnsiColor bg)
    {
        if (!m_HasColor || fg != m_LastFg || bg != m_LastBg)
        {
            m_HasColor = true;
            m_LastFg = fg;
            m_LastBg = bg;

            int fgCode = fg <= AnsiColor.Gray ? 30 + (int)fg : 90 + ((int)fg - 60);
            int bgCode = bg <= AnsiColor.Gray ? 40 + (int)bg : 100 + ((int)bg - 60);

            m_StringBuilder.Append("\x1b[");
            m_StringBuilder.Append(fgCode);
            m_StringBuilder.Append(';');
            m_StringBuilder.Append(bgCode);
            m_StringBuilder.Append('m');
        }
    }
}
