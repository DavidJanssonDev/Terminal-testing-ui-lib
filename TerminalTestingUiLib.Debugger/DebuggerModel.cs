using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalTestingUiLib.Debugger;

internal sealed class DebuggerModel
{
    public double CpuTotalPct { get; private set; }
    public double[] CpuCoresPct { get; private set; } = [];

    public double ManagedMemMb { get; private set; }
    public double WorkingSetMb { get; private set; }

    public int Gc0 { get; private set; }
    public int Gc1 { get; private set; }
    public int Gc2 { get; private set; }

    public int GcDelta0 { get; private set; }
    public int GcDelta1 { get; private set; }
    public int GcDelta2 { get; private set; }


    // Log ring buffer
    private readonly List<string> _logs = [];
    public IReadOnlyList<string> Logs => _logs;

    // History for graphs (fixed size)
    private const int HistoryMax = 120;
    public readonly List<double> ManagedMemHistory = new();
    public readonly List<double> WorkingSetHistory = new();
    public readonly List<int> GcDelta0History = new();
    public readonly List<int> GcDelta1History = new();
    public readonly List<int> GcDelta2History = new();

    public void AddLog(string line)
    {
        _logs.Add(line);
        if (_logs.Count > 2000)
        {
            _logs.RemoveRange(0, 500);
        }
    }

    public void SetCpuTotal(double pct) => CpuTotalPct = pct;

    public void SetCpuCores(double[] cores) => CpuCoresPct = cores;

    public void SetManagedMem(double mb)
    {
        ManagedMemMb = mb;
        PushHistory(ManagedMemHistory, mb);
    }

    public void SetWorkingSet(double mb)
    {
        WorkingSetMb = mb;
        PushHistory(WorkingSetHistory, mb);
    }

    public void SetGcCounts(int g0, int g1, int g2)
    {
        Gc0 = g0;
        Gc1 = g1;
        Gc2 = g2;
    }

    public void SetGcDeltas(int d0, int d1, int d2)
    {
        GcDelta0 = d0;
        GcDelta1 = d1;
        GcDelta2 = d2;

        PushHistory(GcDelta0History, d0);
        PushHistory(GcDelta1History, d1);
        PushHistory(GcDelta2History, d2);
    }

    private static void PushHistory(List<double> list, double v)
    {
        list.Add(v);
        if (list.Count > HistoryMax)
        {
            list.RemoveAt(0);
        }
    }

    private static void PushHistory(List<int> list, int v)
    {
        list.Add(v);
        if (list.Count > HistoryMax)
        {
            list.RemoveAt(0);
        }
    }
}
