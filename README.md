# WinCheck

[English](README.en.md)

WinCheck ist ein Kommandozeilenprogramm in C# für .NET 10. Es prüft den freien Speicherplatz und den Status ausgewählter Windows-Dienste. Die Ergebnisse erscheinen in der Konsole und in einem lokalen JSON-Bericht. Ein Exitcode macht das Ergebnis in Skripten auswertbar.

WinCheck liest den aktuellen Zustand des Rechners. Es startet keine Dienste, ändert keine Systemeinstellungen und führt keine Reparaturen aus.

## Demo ausprobieren

Die Demo vom Repository-Stamm aus mit dem .NET 10 SDK starten.

```powershell
dotnet run --project src/WinCheck -- --demo --output reports/demo.json
```

Mit den mitgelieferten Einstellungen meldet die Demo anhand fester Beispieldaten eine Warnung und liefert Exitcode 1. [JSON-Beispielbericht](examples/demo.json)

## Auf Windows ausführen

```powershell
dotnet run --project src/WinCheck
```

`settings.json` legt den Mindestfreiplatz und die zu prüfenden Dienstnamen fest. Standardmäßig sind EventLog und LanmanWorkstation enthalten. Mit `--config` oder `--output` lässt sich der jeweilige Pfad ändern. Ein gestoppter Dienst kann bei Bedarf automatisch starten. Fehlt ein Dienst oder ist sein Status nicht lesbar, meldet WinCheck einen Fehler.

| Exitcode | Bedeutung |
|---|---|
| 0 | Alle Prüfungen erfolgreich |
| 1 | Mindestens eine Warnung |
| 2 | Fehler bei einer Prüfung oder der Ausführung |

Reale Berichte können den Rechnernamen und lokale Systeminformationen enthalten. Nutze für öffentliche Beispiele den Demo-Bericht. Wähle einen neuen Ausgabepfad, wenn ältere Berichte erhalten bleiben sollen.

## Tests und Lizenz

`dotnet test tests/WinCheck.Tests` prüft Einstellungen, Grenzwerte und Exitcodes. Die Windows-CI führt außerdem die Demo und eine Prüfung des Runner-Systems aus.

[Release Notes](RELEASE-NOTES.md) · [MIT-Lizenz](LICENSE) · [Drittanbieterhinweise](THIRD-PARTY-NOTICES.md)
