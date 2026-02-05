using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer.Rendering;

internal sealed class DiffTerminalRenderer
{
    private readonly int _width;
    private readonly int _height;

    private readonly StringBuilder _sb;

    // What we last presented
    private readonly Cell[] _previous;
    private bool _initialized;

    // Track last emitted colors to avoid spamming color codes
    private bool _hasColor;
    private AnsiColor _lastFg;
    private AnsiColor _lastBg;

    public DiffTerminalRenderer(int width, int height)
    {
        _width = width;
        _height = height;

        // With batching, output is usually small; still give a decent starting capacity.
        _sb = new StringBuilder(width * height / 2);

        _previous = new Cell[width * height];
        _initialized = false;

        _hasColor = false;
        _lastFg = AnsiColor.Gray;
        _lastBg = AnsiColor.Black;

        Console.Write(Ansi.ClearScreen);
        Console.Write(Ansi.CursorHome);
        Console.Write(Ansi.HideCursor);
    }


    public void Present(FrameBuffer current)
    {
        _sb.Clear();

        ReadOnlySpan<Cell> cur = current.Cells;

        // First frame: force paint everything as one big pass (fast + clean)

        if (!_initialized)
        {
            FullRepaint(cur);
            _initialized = true;

            if (_sb.Length > 0)
                Console.Write(_sb.ToString());

            return;
        }


        // Diff repaint with batching.
        // Strategy:
        // For each row:
        //   Find a run of changed cells: [startX..endX]
        //   Move cursor once to start
        //   Write characters across, emitting color changes only when needed

        for (int y = 0; y < _height; y++)
        {
            int rowStart = y * _width;
            int x = 0;

            while (x < _width)
            {
                int idx = rowStart + x;

                // Skip unchanged cells quickly
                if (_previous[idx] == cur[idx])
                {
                    x++;
                    continue;
                }

                // Start of a changed run
                int startX = x;

                // Extend run while cells are changed
                // (We could also stop on long unchanged gaps, but keep it simple + effective.)
                x++;
                while (x < _width)
                {
                    int runIdx = rowStart + x;
                    if (_previous[runIdx] == cur[runIdx])
                        break;
                    x++;
                }

                int endXExclusive = x; // run is [startX, endExclusive]

                // Move cursor once for the run
                Ansi.AppendMoveCursor(_sb, startX, y);

                // Write the run
                for (int xx = startX; xx < endXExclusive; xx++)
                {
                    int runIdx = rowStart + xx;
                    Cell newCell = cur[runIdx];

                    AppendColorIfNeeded(newCell.Fg, newCell.Bg);
                    _sb.Append(newCell.Ch);

                    // Update previous
                    _previous[runIdx] = newCell;
                }
            }
        }

        if (_sb.Length > 0)
            Console.Write(_sb.ToString());
        
    }


    public void Shutdown()
    {
        Console.Write(Ansi.Reset);
        Console.Write(Ansi.ShowCursor);
    }

    private void FullRepaint(ReadOnlySpan<Cell> cur)
    {
        // Reset terminal cursor and then write all cells row-by-row.
        // This is faster than cursor-moving for every cell on the first paint.
        _sb.Append(Ansi.CursorHome);

        // Reset our internal color tracking to ensure correct first output
        _hasColor = false;
        _lastFg = AnsiColor.Gray;
        _lastBg = AnsiColor.Black;

        for (int y = 0; y < _height; y++)
        {
            int rowStart = y * _width;

            for (int x = 0; x < _width; x++)
            {
                int idx = rowStart + x;
                Cell c = cur[idx];

                AppendColorIfNeeded(c.Fg, c.Bg);
                _sb.Append(c.Ch);

                _previous[idx] = c;
            }

            if (y < _height - 1)
            {
                _sb.Append('\n');
            }
        }
    }


    private void AppendColorIfNeeded(AnsiColor fg, AnsiColor bg)
    {
        if (!_hasColor || fg != _lastFg || bg != _lastBg)
        {
            _hasColor = true;
            _lastFg = fg;
            _lastBg = bg;

            // Foreground: 30-37 (dark), 90-97 (bright)
            // Background: 40-47 (dark), 100-107 (bright)
            int fgCode = fg <= AnsiColor.Gray ? 30 + (int)fg : 90 + ((int)fg - 60);
            int bgCode = bg <= AnsiColor.Gray ? 40 + (int)bg : 100 + ((int)bg - 60);

            _sb.Append("\x1b[");
            _sb.Append(fgCode);
            _sb.Append(';');
            _sb.Append(bgCode);
            _sb.Append('m');
        }
    }
}
