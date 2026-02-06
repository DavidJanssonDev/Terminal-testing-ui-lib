namespace TerminalRendererProject.Rendering;

/// <summary>
/// A singel terminal Cell: one character + two colors
/// </summary>
/// <param name="Ch">The Character that cell is representing</param>
/// <param name="Fg">Fourground color</param>
/// <param name="Bg">Background color</param>
public readonly record struct Cell(char Ch, AnsiColor Fg, AnsiColor Bg);
