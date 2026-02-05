using System;
using System.Text;
using TerminalRenderer.Rendering;
using TerminalRenderer.Ui;
using TerminalRendererProject.Rendering;

namespace TerminalRenderer;

internal static class Program
{
    private const int m_Width = 80;
    private const int m_Height = 25;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        FrameBuffer fb = new FrameBuffer(m_Width, m_Height);
        DiffTerminalRenderer renderer = new DiffTerminalRenderer(m_Width, m_Height);

        // Build UI tree (retained mode)
        Window window = new Window("Step 3 - Retained UI Tree + Layout");

        StackPanel stack = new StackPanel { Spacing = 1 };
        Label label = new Label("Tab to move focus. Enter/Space to click. ESC to exit.");

        Button okButton = new Button("OK");
        Button cancel = new Button("Cancel");

        okButton.Clicked += () =>
        {
            label.Text = "OK clicked at " + DateTime.Now.ToLongTimeString();
        };

        cancel.Clicked += () =>
        {
            label.Text = "Cancel clicked at " + DateTime.Now.ToLongTimeString();
        };

        stack.Children.Add(label);
        stack.Children.Add(okButton);
        stack.Children.Add(cancel);

        window.Children.Add(stack);

        UiApp app = new UiApp(window, fb);
        app.Layout(m_Width, m_Height);

        try
        {
            while (true)
            {
                // Input
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    app.HandleKey(key);
                }

                // Render UI into framebuffer and present diff
                app.Render();
                renderer.Present(fb);
            }
        }
        finally
        {
            renderer.Shutdown();
            Console.Clear();
            Console.CursorVisible = true;
        }
    }
}
