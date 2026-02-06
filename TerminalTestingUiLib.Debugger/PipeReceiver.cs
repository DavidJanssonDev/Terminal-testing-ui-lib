using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using TerminalTestingUiLib.Diagnostics;

namespace TerminalTestingUiLib.Debugger;

internal sealed class PipeReceiver : IDisposable
{
    private readonly ConcurrentQueue<string> _lines = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _thread;

    public PipeReceiver()
    {
        _thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "DebuggerPipeReceiver"
        };
        _thread.Start();
    }

    public bool TryDequeue(out string line) => _lines.TryDequeue(out line!);

    private void Run()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                using var pipe = new NamedPipeClientStream(".", DiagnosticsProtocol.PipeName, PipeDirection.In);
                pipe.Connect(1000);

                using var reader = new StreamReader(pipe, Encoding.UTF8);

                while (pipe.IsConnected && !_cts.IsCancellationRequested)
                {
                    string? line = reader.ReadLine();
                    if (line is null)
                    {
                        break;
                    }

                    _lines.Enqueue(line);

                    // Bound queue so debugger can't explode memory if UI falls behind.
                    if (_lines.Count > 50_000)
                    {
                        while (_lines.TryDequeue(out _)) { }
                    }
                }
            }
            catch
            {
                Thread.Sleep(250);
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
    }
}
