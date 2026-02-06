using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Net.Security;
using System.Text;
using System.Text.Json;

namespace TerminalTestingUiLib.Diagnostics;

public sealed class DiagnosticsHost : IDisposable
{
    private static readonly Lazy<DiagnosticsHost> _lazy = new(() => new DiagnosticsHost());
    public static DiagnosticsHost Instance => _lazy.Value;

    private readonly ConcurrentDictionary<int, StreamWriter> _clients = new();
    private readonly PerfService _perf;
    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _acceptThread;

    private int _clientIdSeq;

    // Optional Keep Nlog lines so future debugger clients can request replay later.
    // Step 1: we just keep it for future use
    private readonly ConcurrentQueue<string> _ring = new();
    private const int RingMax = 500;

    private DiagnosticsHost()
    {
        _perf = new PerfService(this);

        _acceptThread = new Thread(AcceptLoop)
        {
            IsBackground = true,
            Name = "DiagnosticsPipeServer"
        };
        _acceptThread.Start();
    }

    public void Publish (DiagnosticsEvent ev)
    {
        string line = JsonSerializer.Serialize(ev);

        // store to ring buffer for futre "replay on connect" feature
        _ring.Enqueue(line);
        while (_ring.Count > RingMax && _ring.TryDequeue(out _)) { }

        // Brodcast to connected clients
        foreach (KeyValuePair<int, StreamWriter> kv in _clients)
        {
            try
            {
                kv.Value.WriteLine(line);
            }
            catch
            {
                RemoveClient(kv.Key);
            }
        }
    }

    private void AcceptLoop()
    {
        // Accept clients forever. Each Connection gets its own NamedPipeServerStream.
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(
                    pipeName: DiagnosticsProtocol.PipeName,
                    direction: PipeDirection.Out,
                    maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
                    transmissionMode: PipeTransmissionMode.Byte,
                    options: PipeOptions.Asynchronous
                );

                server.WaitForConnection();

                if (_cts.IsCancellationRequested)
                {
                    return;
                }

                var writer = new StreamWriter(server, new UTF8Encoding(false))
                {
                    AutoFlush = true
                };

                int id = Interlocked.Increment(ref _clientIdSeq);
                _clients.TryAdd(id, writer);

                // Step 1: we do not replay the ring buffer yet (we can add in Setp 3 easily)
                // For now, send a small hello event.
                writer.WriteLine(JsonSerializer.Serialize(
                    new DiagnosticsEvent(
                        Type: "log",
                        Utc: DateTime.UtcNow,
                        Level: LogLevel.Info,
                        Category: "Diagnostics",
                        Message: "Debugger attached",
                        Payload: null
                    )
                ));


                // Keep this connection alive until it breaks.
                // We cannot dispose "server" while weiter is in use, so we block here.
                // The easiest pattern: wait untill write fails on broadcast, then RemoveClient.
                while (server.IsConnected && !_cts.IsCancellationRequested)
                {
                    Thread.Sleep(250);
                }

                RemoveClient(id);
            }
            catch
            {
                // If something goes wrong, don't crash the app
                Thread.Sleep(250);
            }
        }
    }

    private void RemoveClient(int id)
    {
        if (_clients.TryRemove(id, out StreamWriter? writer))
        {
            try { writer.Dispose(); } catch { }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();

        try { _perf.Dispose(); } catch { }

        foreach (var kv in _clients)
        {
            try { kv.Value.Dispose(); } catch { }
        }
        _clients.Clear();
    }
}