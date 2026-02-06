namespace TerminalRendererProject.Rendering;

public enum AnsiColor
{
    Black = 0,
    DarkRed = 1,
    DarkGreen = 2,
    DarkYellow = 3,
    DarkBlue = 4,
    DarkMagenta = 5,
    DarkCyan = 6,
    Gray = 7,

    // Bright variants (we map these to 90-97 and 100-107 later)
    DarkGray = 60,
    Red = 61,
    Green = 62,
    Yellow = 63,
    Blue = 64,
    Magenta = 65,
    Cyan = 66,
    White = 67
}