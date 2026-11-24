using System.ComponentModel;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace WinCheck;

public static class SystemChecks
{
    public static CheckResult EvaluateDisk(string name, long totalBytes, long freeBytes, double minimumFreePercent)
    {
        if (totalBytes <= 0 || freeBytes < 0 || freeBytes > totalBytes)
            return new CheckResult(name, "ERROR", "Ungültige Speicherwerte.");

        double freePercent = 100.0 * freeBytes / totalBytes;
        double freeGiB = freeBytes / Math.Pow(1024, 3);
        string status = freePercent < minimumFreePercent ? "WARN" : "OK";
        return new CheckResult(name, status,
            FormattableString.Invariant($"{freeGiB:F1} GiB frei ({freePercent:F1} %), Grenze: {minimumFreePercent} %"));
    }

    public static List<CheckResult> CheckDisks(double minimumFreePercent)
    {
        var results = new List<CheckResult>();
        foreach (var drive in DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Fixed))
        {
            try
            {
                results.Add(!drive.IsReady
                    ? new CheckResult(drive.Name, "ERROR", "Laufwerk ist nicht bereit.")
                    : EvaluateDisk(drive.Name, drive.TotalSize, drive.AvailableFreeSpace, minimumFreePercent));
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                results.Add(new CheckResult(drive.Name, "ERROR", exception.Message));
            }
        }
        if (results.Count == 0)
            results.Add(new CheckResult("Laufwerke", "ERROR", "Keine lokalen Festplatten gefunden."));
        return results;
    }

    [SupportedOSPlatform("windows")]
    public static List<CheckResult> CheckServices(IEnumerable<string> serviceNames)
    {
        var results = new List<CheckResult>();
        foreach (string name in serviceNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                using var service = new ServiceController(name);
                var status = service.Status;
                results.Add(new CheckResult(name, status == ServiceControllerStatus.Running ? "OK" : "WARN",
                    $"Status: {status}"));
            }
            catch (Exception exception) when (exception is InvalidOperationException or Win32Exception)
            {
                results.Add(new CheckResult(name, "ERROR", exception.Message));
            }
        }
        return results;
    }
}
