# WinCheck

[Deutsch](README.md)

WinCheck is a C#/.NET 10 CLI that checks free disk space and the status of selected Windows services. It prints the results, writes a local JSON report and returns an exit code for scripts.

WinCheck reads the current state of the machine. It does not start services, change system settings or perform repairs.

## Try the demo

From the repository root, run the demo with the .NET 10 SDK.

```powershell
dotnet run --project src/WinCheck -- --demo --output reports/demo.json
```

The demo uses fixed sample data. With the included settings it reports a warning and exits with code 1. [Sample JSON report](examples/demo.json)

## Run on Windows

```powershell
dotnet run --project src/WinCheck
```

`settings.json` sets the free-space threshold and the Windows service names to check. The defaults include EventLog and LanmanWorkstation. Use `--config` or `--output` to choose a different file. A stopped service that starts on demand can be normal. A missing or unreadable service is reported as an error.

| Exit code | Meaning |
|---|---|
| 0 | All checks passed |
| 1 | A check reported a warning |
| 2 | A check or the run failed |

A real report can contain the computer name and local system details. Use the demo report for public examples. Choose a different output path when you want to keep an earlier report.

## Tests and license

`dotnet test tests/WinCheck.Tests` covers settings, thresholds and exit codes. The Windows workflow also runs the demo and the live checks on its runner.

[Release notes](RELEASE-NOTES.md) · [MIT license](LICENSE) · [Third-party notices](THIRD-PARTY-NOTICES.md)
