using System.Text;
using System.Text.Json;
using TerminalRenderer.Rendering;
using TerminalRenderer.UI;
using TerminalRendererProject.Rendering;
using TerminalTestingUiLib.Debugger.Controls;

namespace TerminalTestingUiLib.Debugger;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        int width = Math.Max(40, Console.WindowWidth);
        int height = Math.Max(15, Console.WindowHeight);

        var fb = new FrameBuffer(width, height);
        var renderer = new DiffTerminalRenderer(width, height);

        var model = new DebuggerModel();
        using var receiver = new PipeReceiver();

        // UI controls
        var status = new StatusBar();

        var logView = new LogView(model);
        var cpuGrid = new CpuCoreGrid(model);

        var memGraph = new BarGraph
        {
            Title = "Managed Memory (MB)",
            GetSeries = () => model.ManagedMemHistory
        };

        var wsGraph = new BarGraph
        {
            Title = "Working Set (MB)",
            GetSeries = () => model.WorkingSetHistory
        };

        var gcGraph = new GcGraph(model);

        var rightStack = new StackPanel
        {
            Spacing = 1,
            Children =
            {
                cpuGrid,
                memGraph,
                wsGraph,
                gcGraph
            }
        };

        var split = new SplitPanel
        {
            LeftRatio = 0.52,
            Children =
            {
                logView,
                rightStack
            }
        };

        Control root =
            new Window("Terminal UI Debugger")
            {
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 1,
                        Children =
                        {
                            status,
                            split
                        }
                    }
                }
            };

        var app = new UiApp(root, fb);
        app.Layout(width, height);

        try
        {
            while (true)
            {
                // ✅ Detect terminal resize
                int newW = Math.Max(40, Console.WindowWidth);
                int newH = Math.Max(15, Console.WindowHeight);

                if (newW != width || newH != height)
                {
                    width = newW;
                    height = newH;

                    // Recreate framebuffer + renderer for new size
                    fb = new FrameBuffer(width, height);
                    renderer.Shutdown();
                    renderer = new DiffTerminalRenderer(width, height);

                    // Rebind UiApp to new framebuffer
                    app = new UiApp(root, fb);
                    app.Layout(width, height);

                    // Force a repaint
                    app.InvalidateLayout();
                }

                // Drain incoming pipe lines
                bool changed = false;
                while (receiver.TryDequeue(out string line))
                {
                    changed |= ApplyEvent(model, line);
                }

                // Update status bar text
                status.LeftText = $"CPU {model.CpuTotalPct:0.0}%  MEM {model.ManagedMemMb:0.0}MB  WS {model.WorkingSetMb:0.0}MB";
                status.RightText = $"GC {model.Gc0}/{model.Gc1}/{model.Gc2}  Δ {model.GcDelta0}/{model.GcDelta1}/{model.GcDelta2}";

                if (changed)
                {
                    app.InvalidateVisual();
                }

                // Input (scroll logs + exit)
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    if (key.Key == ConsoleKey.UpArrow) { logView.ScrollLines(1); app.InvalidateVisual(); }
                    if (key.Key == ConsoleKey.DownArrow) { logView.ScrollLines(-1); app.InvalidateVisual(); }
                    if (key.Key == ConsoleKey.PageUp) { logView.ScrollPages(1); app.InvalidateVisual(); }
                    if (key.Key == ConsoleKey.PageDown) { logView.ScrollPages(-1); app.InvalidateVisual(); }
                    if (key.Key == ConsoleKey.End) { logView.ScrollToBottom(); app.InvalidateVisual(); }
                }

                if (app.Tick())
                {
                    renderer.Present(fb);
                }
                else
                {
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

    private static bool ApplyEvent(DebuggerModel model, string jsonLine)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonLine);
            var root = doc.RootElement;

            string type = root.GetProperty("Type").GetString() ?? "";
            string category = root.GetProperty("Category").GetString() ?? "";
            string message = root.GetProperty("Message").GetString() ?? "";

            if (type == "log")
            {
                model.AddLog($"[{category}] {message}");
                return true;
            }

            if (type == "metric")
            {
                var payload = root.GetProperty("Payload");
                double value = payload.GetProperty("Value").GetDouble();

                switch (message)
                {
                    case "cpu.total_pct":
                        model.SetCpuTotal(value);
                        return true;

                    case "mem.managed_mb":
                        model.SetManagedMem(value);
                        return true;

                    case "mem.process_working_set_mb":
                        model.SetWorkingSet(value);
                        return true;

                    case "gc.gen0":
                        model.SetGcCounts((int)value, model.Gc1, model.Gc2);
                        return true;

                    case "gc.gen1":
                        model.SetGcCounts(model.Gc0, (int)value, model.Gc2);
                        return true;

                    case "gc.gen2":
                        model.SetGcCounts(model.Gc0, model.Gc1, (int)value);
                        return true;

                    case "gc.delta.gen0":
                        model.SetGcDeltas((int)value, model.GcDelta1, model.GcDelta2);
                        return true;

                    case "gc.delta.gen1":
                        model.SetGcDeltas(model.GcDelta0, (int)value, model.GcDelta2);
                        return true;

                    case "gc.delta.gen2":
                        model.SetGcDeltas(model.GcDelta0, model.GcDelta1, (int)value);
                        return true;
                }
            }

            if (type == "snapshot" && message == "cpu.cores_pct")
            {
                var payload = root.GetProperty("Payload");
                var values = payload.GetProperty("values");

                double[] cores = new double[values.GetArrayLength()];
                for (int i = 0; i < cores.Length; i++)
                {
                    cores[i] = values[i].GetDouble();
                }

                model.SetCpuCores(cores);
                return true;
            }
        }
        catch
        {
            model.AddLog(jsonLine);
            return true;
        }

        return false;
    }
}
