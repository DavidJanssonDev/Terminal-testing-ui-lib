using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalRenderer.UI;

internal readonly record struct Size(int Width, int Height);
internal readonly record struct Rect(int X, int Y, int Width, int Height);
