# Terminal Testing UI Library

A C#/.NET **ANSI-based terminal UI library** that renders a retained-mode control tree into a character framebuffer and efficiently presents updates using diff rendering.

This project is both a **learning-focused exploration of rendering and UI architecture** and a **practical foundation** for building real terminal-based applications.

> ⚠️ Status: **Early development / experimental**  
> APIs and internal architecture may change as the project evolves.

---

## Key Features

- ANSI-based terminal rendering (Windows Terminal recommended)
- Cell framebuffer (`char + foreground + background`)
- Double buffering with **diff rendering and run batching**
- Retained-mode UI tree (controls as classes, DOM-like)
- Layout system with `Measure` / `Arrange` passes
- Keyboard input handling and focus navigation
- Two ways to construct UIs:
  - Pure class-based object tree (recommended)
  - Optional DSL / factory (syntax sugar only)

---

## Requirements

- Windows 10/11
- Windows Terminal or any ANSI-capable terminal
- .NET 8 SDK (or .NET 6+)

---

## Getting Started

### Build

```bash
dotnet build
```

### Run demo

```bash
dotnet run
```

---

## Example Usage

### Class-based (DOM-like, recommended)

```csharp
var label = new Label("Ready.");

Control root =
    new Window("Terminal UI")
    {
        Children =
        {
            new StackPanel
            {
                Children =
                {
                    label,
                    new Button("OK")
                    {
                        OnClick = () => label.Text = "OK clicked"
                    },
                    new Button("Cancel")
                    {
                        OnClick = () => label.Text = "Cancel clicked"
                    }
                }
            }
        }
    };
```

### Optional DSL / Factory (syntax sugar)

```csharp
Control root =
    Ui.Window("Terminal UI",
        Ui.Stack(
            Ui.Label("Hello"),
            Ui.Button("OK", () => Console.Beep())
        )
    );
```

Both approaches produce the **same retained UI tree** and have **no runtime performance difference**.

---

## Architecture Overview

### Rendering Pipeline

1. UI controls render into a `FrameBuffer`
2. Each cell contains:
   - character
   - foreground color
   - background color
3. The `DiffTerminalRenderer` compares the current frame with the previous frame
4. Only changed *runs* of cells are emitted using ANSI cursor and color sequences

### UI Pipeline

- `Measure(availableSize)` → compute desired size
- `Arrange(finalRect)` → assign layout bounds
- `Render(frameBuffer)` → draw into framebuffer
- `OnKey(key)` → handle input (focused control)

This mirrors the architecture of retained-mode UI frameworks (e.g. WPF, XAML, DOM-based systems), adapted for terminal rendering.

---

## Project Structure

```txt
Rendering/
  - FrameBuffer, Cell, Draw helpers
  - ANSI backend and diff renderer

UI/
  - Control base class
  - Layout containers (StackPanel, Window)
  - Controls (Label, Button)
  - Focus and input coordinator (UiApp)

Program.cs
  - Demo application / test harness
```

---

## Roadmap

- [ ] Dirty invalidation (render only when needed)
- [ ] TextBox control (caret, editing, selection)
- [ ] Styling system (ClassName-based, themes)
- [ ] Additional layouts (horizontal, grid)
- [ ] Scrollable containers
- [ ] Mouse support (terminal-dependent)
- [ ] NuGet packaging

---

## Contributing

Contributions are welcome.

This project is intentionally exploratory, so discussions and design feedback are encouraged.

See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

---

## License

MIT License © David Jansson  
See [LICENSE](LICENSE).
