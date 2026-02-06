using System.Text;
using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;
using TerminalTestingUiLib.Diagnostics;

namespace TerminalRenderer;

internal static class Program
{
    private const int Width = 80;
    private const int Height = 25;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Rendering
        var fb = new FrameBuffer(Width, Height);
        var renderer = new DiffTerminalRenderer(Width, Height);

        // UI controls
        var label = new Label("Ready.");

        var okButton = new Button("OK");
        var cancelButton = new Button("Cancel");

        Control root =
            new Window("Terminal UI – Invalidation + Diagnostics")
            {
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 1,
                        Children =
                        {
                            label,
                            okButton,
                            cancelButton
                        }
                    }
                }
            };

        // UI application
        var app = new UiApp(root, fb);
        app.Layout(Width, Height);

        // Wire events AFTER app exists (so we can invalidate + log)
        okButton.OnClick = () =>
        {
            label.Text = "OK clicked";
            app.InvalidateVisual();
            Log.Info("BUTTONS","OK BUTTON CLICKED");
        };

        cancelButton.OnClick = () =>
        {
            label.Text = "Cancel clicked";
            app.InvalidateVisual();
            Log.Info("BUTTONS","CANCEL BUTTON CLICKED");
        };


        Log.Info("App", "Terminal Started");
        try
        {
            while (true)
            {


                // Handle input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    app.HandleKey(key);
                }

                // Only render/present when something changed
                if (app.Tick())
                {
                    renderer.Present(fb);
                }
                else
                {
                    // Idle: avoid burning CPU
                    Thread.Sleep(10);
                }
            }
        }
        finally
        {
            renderer.Shutdown();
            Console.Clear();
        }
    }
}
