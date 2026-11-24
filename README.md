# WinCheck

WinCheck ist ein C#/.NET-10-Kommandozeilenprogramm für zwei wiederkehrende Windows-Prüfungen. Es meldet knappen Speicherplatz und prüft, ob ausgewählte Dienste laufen. Ergebnisse stehen in der Konsole und in einem lokalen JSON-Bericht.

## Demo und Systemlauf

Vom Repository-Stamm aus mit .NET 10:

```powershell
dotnet run --project src/WinCheck -- --demo --output reports/demo.json
```

Die Demo nutzt feste Beispielwerte. Mit der mitgelieferten Konfiguration meldet sie absichtlich eine Warnung und endet mit Exitcode 1. Auf Windows liefert der Aufruf ohne `--demo` reale Daten.

```powershell
dotnet run --project src/WinCheck
```

`settings.json` enthält Mindestfreiplatz in Prozent und die ausgewählten Dienste. `--config` und `--output` überschreiben die Pfade. Die Standardkonfiguration prüft EventLog und LanmanWorkstation anhand ihrer Windows-Dienstnamen. Laufende Dienste ergeben OK. Andere Status ergeben WARN. Fehlende oder nicht lesbare Dienste ergeben ERROR. Ein gestoppter bedarfsgesteuerter Dienst kann beabsichtigt sein. Die Prüfergebnisse dienen der Diagnose.

| Exitcode | Bedeutung |
|---|---|
| 0 | Prüfungen ohne Warnung oder Fehler |
| 1 | Mindestens eine Warnung |
| 2 | Fehler bei Prüfung oder Ausführung |

## Tests und Berichte

`dotnet test tests/WinCheck.Tests` prüft unter anderem Schwellen, Konfiguration und Exitcodes. Der Windows-Workflow sieht zusätzlich einen Demo- und einen Systemlauf vor.

Der reale Bericht enthält unter anderem den Rechnernamen und lokale Zustände. Für Beispiele den Demo-Bericht verwenden. Einen eigenen Ausgabepfad für jeden Lauf wählen, wenn frühere JSON-Berichte erhalten bleiben sollen.


[Beispielbericht](examples/demo.json) · [MIT-Lizenz](LICENSE)

[Release Notes](RELEASE-NOTES.md) · [Drittanbieterhinweise](THIRD-PARTY-NOTICES.md)
