using System;

namespace TerminalRendererProject.Rendering;

internal sealed class FrameBuffer
{
    private readonly Cell[] m_CellList;

    public int Width { get; }
    public int Height { get; }

    public FrameBuffer(int width, int height)
    {
        Width = width;
        Height = height;
        m_CellList = new Cell[Width * Height];
    }

    public void Clear(Cell cell)
    {
        Array.Fill(m_CellList, cell);
    }

    public void Set(int xCord, int yCord, Cell cell)
    {
        // Using uint trick avoids two comparisons pre bound check
        if ((uint)xCord >= (uint)Width || (uint)yCord >= (uint)Height)
        {
            return;
        }

        m_CellList[(yCord * Width) + xCord] = cell;
    }

    public ReadOnlySpan<Cell> Cells => m_CellList;
}
