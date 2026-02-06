using System.Diagnostics;

namespace TerminalTestingUiLib.Diagnostics;

internal sealed class WindowsPerfCountersSampler : IDisposable
{
    private readonly PerformanceCounter _total;
    private readonly PerformanceCounter[] _cores;

    private bool _warmedUp;

    public WindowsPerfCountersSampler()
    {
        PerformanceCounterCategory category = new ("Processor");
        string[] instances = category.GetInstanceNames();

        // Core instances are usually "0", "1", ... plus "_Total"
        var coreInstances = instances
            .Where(n => n != "_Total")
            .Select(n =>
            {
                // Only accept numeric core instance names
                return int.TryParse(n, out _) ? n : null;
            })
            .Where(n => n is not null)
            .Cast<string>()
            .OrderBy(n => int.Parse(n))
            .ToArray();

        _total = new PerformanceCounter("Processor", "% Processor Time", "_Total", readOnly: true);
        _cores = coreInstances
            .Select(inst => new PerformanceCounter("Processor", "% Processor Time", inst, readOnly: true))
            .ToArray();

        // Warm up counters: first read is often 0 or garbage until next sample interval.
        _ = _total.NextValue();
        foreach (var c in _cores)
        {
            _ = c.NextValue();
        }

        _warmedUp = false;
    }

    public (double total, double[] cores)? Sample()
    {
        // After warmup, second sample is meaningful.
        double total = _total.NextValue();
        double[] cores = new double[_cores.Length];

        for (int i = 0; i < _cores.Length; i++)
        {
            cores[i] = _cores[i].NextValue();
        }

        if (!_warmedUp)
        {
            _warmedUp = true;
            return null;
        }

        // Clamp to sane ranges
        total = ClampPercent(total);
        for (int i = 0; i < cores.Length; i++)
        {
            cores[i] = ClampPercent(cores[i]);
        }

        return (total, cores);
    }

    private static double ClampPercent(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v))
        {
            return 0;
        }

        if (v < 0) return 0;
        if (v > 100) return 100;
        return v;
    }

    public void Dispose()
    {
        try { _total.Dispose(); } catch { }
        foreach (var c in _cores)
        {
            try { c.Dispose(); } catch { }
        }
    }
}
