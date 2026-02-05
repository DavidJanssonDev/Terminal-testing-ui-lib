using System;
using System.Text;
using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer;

internal static class Program
{
    private const int Width = 80;
    private const int Height = 25;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var fb = new FrameBuffer(Width, Height);
        var renderer = new DiffTerminalRenderer(Width, Height);

        var label = new Label("Ready.");

        Control root =
            new Window("Terminal UI – Class-Based DOM")
            {
                Children =
                {
                    new StackPanel
                    {
                        Children =
                        {
                            label,
                            new Button("OK") { OnClick = () => label.Text = "OK clicked" },
                            new Button("Cancel") { OnClick = () => label.Text = "Cancel clicked" }
                        }
                    }
                }
            };

        var app = new UiApp(root, fb);
        app.Layout(Width, Height);

        try
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape) break;
                    app.HandleKey(key);
                }

                app.Render();
                renderer.Present(fb);
            }
        }
        finally
        {
            renderer.Shutdown();
            Console.Clear();
        }
    }
}
