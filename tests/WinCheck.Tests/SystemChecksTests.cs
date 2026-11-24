using WinCheck;

namespace WinCheck.Tests;

public class SystemChecksTests
{
    [Theory]
    [InlineData(14, "WARN")]
    [InlineData(15, "OK")]
    [InlineData(16, "OK")]
    public void DiskWarningDependsOnFreeSpace(long freeBytes, string expectedStatus)
    {
        var result = SystemChecks.EvaluateDisk("C:", 100, freeBytes, 15);
        Assert.Equal(expectedStatus, result.Status);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(100, -1)]
    [InlineData(100, 101)]
    public void InvalidDiskValuesProduceAnError(long totalBytes, long freeBytes)
    {
        var result = SystemChecks.EvaluateDisk("C:", totalBytes, freeBytes, 15);
        Assert.Equal("ERROR", result.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(-5)]
    [InlineData(double.NaN)]
    public void InvalidThresholdIsRejected(double threshold)
    {
        var settings = new Settings { MinimumFreePercent = threshold };
        Assert.Throws<InvalidDataException>(settings.Validate);
    }

    [Fact]
    public void EmptyServiceNameIsRejected()
    {
        var settings = new Settings { Services = ["EventLog", " "] };
        Assert.Throws<InvalidDataException>(settings.Validate);
    }

    [Fact]
    public void ServiceChecksCanBeDisabled()
    {
        var settings = new Settings { Services = [] };
        settings.Validate();
    }

    [Theory]
    [InlineData("OK", "OK", 0)]
    [InlineData("WARN", "OK", 1)]
    [InlineData("WARN", "ERROR", 2)]
    [InlineData("ERROR", "WARN", 2)]
    public void ErrorsTakePriorityOverWarnings(string first, string second, int expectedExitCode)
    {
        var report = new CheckReport(DateTimeOffset.Now, "TEST-PC",
            [new CheckResult("C:", first, "Test"), new CheckResult("EventLog", second, "Test")]);
        Assert.Equal(expectedExitCode, report.ExitCode);
    }
}
