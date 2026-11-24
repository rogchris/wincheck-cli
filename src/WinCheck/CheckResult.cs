namespace WinCheck;

public record CheckResult(string Name, string Status, string Detail);

public record CheckReport(DateTimeOffset CheckedAt, string Computer, List<CheckResult> Results)
{
    public int ExitCode => Results.Any(result => result.Status == "ERROR") ? 2
        : Results.Any(result => result.Status == "WARN") ? 1 : 0;
}
