using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.UI;

public readonly record struct Size(int Width, int Height);
public readonly record struct Rect(int X, int Y, int Width, int Height);
