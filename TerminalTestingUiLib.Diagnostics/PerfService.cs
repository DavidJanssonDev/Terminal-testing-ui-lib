using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalTestingUiLib.Diagnostics;

internal sealed class PerfService : IDisposable
{
    private readonly DiagnosticsHost _host;
    private readonly WindowsPerfCountersSampler _cpuSampler;

    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _thread;

    private int _lastG0;
    private int _lastG1;
    private int _lastG2;

    public PerfService(DiagnosticsHost host)
    {
        _host = host;
        _cpuSampler = new WindowsPerfCountersSampler();

        _lastG0 = GC.CollectionCount(0);
        _lastG1 = GC.CollectionCount(1);
        _lastG2 = GC.CollectionCount(2);

        _thread = new Thread(Loop)
        {
            IsBackground = true,
            Name = "DiagnosticPerfService"
        };

        _thread.Start();
    }



    private void Loop()
    {
        // Publish about twice per second (nice for graphs)
        const int IntervalMs = 500;

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var cpu = _cpuSampler.Sample();
                
                // Always Collect memory/GC; Cpu may be warming up
                double managedMb = GC.GetTotalMemory(forceFullCollection: false) / 1024.0 / 1024.0;

                // Working set is a useful "real footprint" graph.
                double workingSetMb = Environment.WorkingSet / 1024.0 / 1024.0;

                int g0 = GC.CollectionCount(0);
                int g1 = GC.CollectionCount(1);
                int g2 = GC.CollectionCount(2);

                if (cpu is not null)
                {
                    PublishMetric("cpu.total_pct", cpu.Value.total, "%");
                    PublishSnapshot("cpu.cores_pct", cpu.Value.cores, "%");
                }

                PublishMetric("mem.managed_mb", managedMb, "MB");
                PublishMetric("mem.process_working_set_mb", workingSetMb, "MB");

                // Publish cumulative counts (good for graphs)
                PublishMetric("gc.gen0", g0, "count");
                PublishMetric("gc.gen1", g1, "count");
                PublishMetric("gc.gen2", g2, "count");


                // Also publish per-interval deltas (good for "rate" graphs)
                int d0 = g0 - _lastG0;
                int d1 = g1 - _lastG1;
                int d2 = g2 - _lastG2;

                _lastG0 = g0;
                _lastG1 = g1;
                _lastG2 = g2;

                PublishMetric("gc.delta.gen0", d0, "count");
                PublishMetric("gc.delta.gen1", d1, "count");
                PublishMetric("gc.delta.gen2", d2, "count");
            }
            catch
            {
                // Never let diagnostics crash the app.
            }

            Thread.Sleep(IntervalMs);
        }
    }

    private void PublishMetric(string name, double value, string unit)
    {
        _host.Publish(new DiagnosticsEvent(
            Type: "metric",
            Utc: DateTime.UtcNow,
            Level: null,
            Category: "Perf",
            Message: name,
            Payload: new MetricPayload(value, unit)
        ));
    }

    private void PublishMetric(string name, int value, string unit)
       => PublishMetric(name, (double)value, unit);

    private void PublishSnapshot(string name, double[] values, string unit)
    {
        // Snapshot as payload array + unit (future-proof for UI)
        _host.Publish(new DiagnosticsEvent(
            Type: "snapshot",
            Utc: DateTime.UtcNow,
            Level: null,
            Category: "Perf",
            Message: name,
            Payload: new { values, unit }
        ));
    }

    public void Dispose()
    {
        _cts.Cancel();
        try { _cpuSampler.Dispose(); } catch { }
    }
}
