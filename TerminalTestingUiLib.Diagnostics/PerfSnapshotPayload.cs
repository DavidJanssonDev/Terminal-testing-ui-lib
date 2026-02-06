namespace TerminalTestingUiLib.Diagnostics;

public sealed record PerfSnapshotPayload(
    double CpuTotalPercent,
    double[] CpuCoresPercent,
    double ManagedMemoryMb,
    double WorkingSetMb,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections
);
