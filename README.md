# EasyTestAufgabe

Mini Project & Task Tracker — Testaufgabe im Rahmen der Bewerbung als Softwareentwickler:in .NET/C# bei der **EasyCode-IT AG**.

**Autor:** Oleksandr (Alex) Telnov  
**Entwicklungszeitraum:** 18.–19. August 2026 (Vorgabe: 1 Arbeitstag)  
**Repository:** https://github.com/telnovxander/EasyTestAufgabe

---

## Tech-Stack

- ASP.NET Core 9, Blazor Web App (Interactivity: Server)
- SQLite über EF Core (Code First, Migrations)
- Serilog (Konsole + rollierende Datei, zusätzliche Logs-Ansicht im UI unter `/logs`)
- xUnit + EF Core InMemory Provider (Unit-Tests)

---

## Anforderungen — Umsetzungsstatus

| # | Anforderung | Status |
|---|---|---|
| 1 | Projekte anlegen, bearbeiten, löschen (Name, Kunde, Status) | ✅ |
| 2 | Aufgaben pro Projekt (Titel, Beschreibung, Status, Priorität) | ✅ |
| 3 | Zeiteinträge pro Aufgabe (Datum, Dauer, Notiz) | ✅ |
| 4 | Projekt-Übersicht mit Gesamtzeit sowie Anzahl offener/erledigter Aufgaben | ✅ |
| 5 | (Bonus) Aufgaben nach Status oder Freitext filtern/suchen | ✅ |
| — | (Zusatz) Abgeschlossene Projekte sind schreibgeschützt (Tasks/Zeiteinträge) | ✅ |
| — | (Zusatz) Request- und CRUD-Logging via Serilog inkl. Logs-Ansicht | ✅ |

Alle Punkte lokal getestet — inklusive frischem `git clone` in ein leeres Verzeichnis, um sicherzustellen, dass die Anleitung unten tatsächlich reproduzierbar funktioniert.

---

## Lokal starten

Voraussetzung: [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Über die Konsole (PowerShell)

```powershell
git clone https://github.com/telnovxander/EasyTestAufgabe.git
cd EasyTestAufgabe

dotnet restore

dotnet ef database update `
  --project src/EasyTestAufgabe.Infrastructure `
  --startup-project src/EasyTestAufgabe.Web

dotnet run --project src/EasyTestAufgabe.Web
```

Die Konsole zeigt beim Start eine Zeile wie:
```
Now listening on: https://localhost:XXXX
```
Der tatsächliche Port variiert je nach Rechner/Umgebung — **die in der eigenen Konsolenausgabe angezeigte URL** im Browser öffnen, nicht eine fest angenommene. Beim ersten Start wird die SQLite-Datenbank automatisch migriert und mit Demo-Daten befüllt (3 Projekte, mehrere Aufgaben und Zeiteinträge).

Falls `dotnet ef` nicht gefunden wird:
```powershell
dotnet tool install --global dotnet-ef
```

### Alternativ über Visual Studio

1. Repository klonen — entweder über die Konsole (`git clone https://github.com/telnovxander/EasyTestAufgabe.git`) oder direkt aus Visual Studio: Startfenster → **Code auschecken** (*Clone a repository*) → Repository-URL `https://github.com/telnovxander/EasyTestAufgabe.git` einfügen → lokalen Pfad wählen → **Klonen**
2. `EasyTestAufgabe.sln` im geklonten Ordner öffnen (öffnet sich nach dem Klonen meist automatisch)
3. Im **Solution Explorer** prüfen, dass **EasyTestAufgabe.Web** als Startprojekt gesetzt ist (fett dargestellt). Falls nicht: Rechtsklick auf `EasyTestAufgabe.Web` → *Als Startprojekt festlegen*
4. **F5** drücken (oder den grünen Play-Button/"Start" in der Toolbar)
5. Visual Studio öffnet automatisch einen Browser mit der Anwendung; die verwendete Adresse/Port ist ebenfalls im Ausgabefenster ("Output") ersichtlich

Die Datenbank wird beim ersten Start automatisch migriert und befüllt — in Visual Studio ist dafür kein manueller EF-Core-Schritt nötig.

---

## Architektur

```
src/
├── EasyTestAufgabe.Web/            Blazor Server UI
│   ├── Components/Pages/           Seiten (Projekte, Aufgaben, Zeiterfassung, Logs)
│   ├── Components/Shared/          Wiederverwendbare Komponenten (FormModal, ConfirmDialog)
│   ├── Models/                     Präsentationsmodelle für Formulare (DataAnnotations)
│   ├── Helpers/                    Formatierung, Enum-Anzeigetexte
│   └── Program.cs
├── EasyTestAufgabe.Application/    Services, DTOs, Validierung (Result-Pattern)
├── EasyTestAufgabe.Domain/         Entitäten (Project, TaskItem, TimeEntry), Enums
└── EasyTestAufgabe.Infrastructure/ EF Core DbContext, Migrations, Seed-Daten

tests/
└── EasyTestAufgabe.Tests/          Unit-Tests der Application-Services
```

Bewusste Vereinfachung gegenüber "reiner" Clean Architecture: `Application` referenziert `Infrastructure` direkt (statt über Repository-Interfaces in `Domain`), da der Umfang dieses Projekts eine zusätzliche Abstraktionsebene nicht rechtfertigt.

---

## Tests ausführen

In Visual Studio: **Test** → **Run All Tests** (bzw. Test-Explorer öffnen, <kbd>Strg</kbd>+<kbd>R</kbd>, <kbd>A</kbd>).

13 Unit-Tests decken die Kernlogik der drei Application-Services ab: Aggregation der Projekt-Statistik, zentrale Validierungsregeln sowie die Geschäftsregel "abgeschlossenes Projekt ist schreibgeschützt".

---

## Anmerkungen zur Umsetzung

Ein paar Entscheidungen, die ich bewusst getroffen habe und gerne im Gespräch näher erläutere:

- **Blazor statt React:** einfachere Integration mit .NET, keine separate Infrastruktur für einen React-Build/-Server nötig. Für den Umfang dieser Aufgabe war das ausreichend — den zusätzlichen Setup-Aufwand eines JS-Frontends hielt ich hier für nicht gerechtfertigt.
- **SQLite statt eines "echten" Datenbankservers:** aus demselben Grund — einfaches, serverloses lokales Setup. Aus demselben Pragmatismus heraus referenziert `Application` `Infrastructure` direkt, ohne Repository-Interfaces dazwischen (siehe Architektur oben).
- **.NET 9 statt .NET 8 (LTS) oder .NET 10 (aktuelles LTS):** Auf meiner Maschine war Visual Studio 2022 installiert, das .NET 9 vollständig unterstützt (für .NET 10 wäre Visual Studio 2026 nötig gewesen). Der Support-Zeitraum von .NET 9 läuft bis November 2026 — für den Zweck dieser Aufgabe ausreichend.
- **Logging als bewusster Umfang über die Anforderungen hinaus:** Ich habe mich gegen eine Demo von Exception-Handling entschieden, da sich das in einer Demo nicht seriös zeigen lässt (man müsste künstlich einen Fehler provozieren). Request- und CRUD-Logging via Serilog halte ich dagegen für ein Muss in jedem realen Projekt — es hat mir in früheren Projekten wiederholt geholfen, daher habe ich es hier bewusst mit eingebaut.
- **Deutsche Umlaute in Code-Kommentaren:** Ich habe mir darüber keine grossen Gedanken gemacht und keine feste Konvention durchgezogen. Mich würde interessieren, welche Code-Konventionen im Team hierzu gelten (z. B. reines UTF-8 oder ASCII-Transliteration wie "moeglich" statt "möglich").
- **Keine Authentifizierung und kein Deployment:** Die Abgabe erfolgt gemäss Aufgabenstellung als lokal lauffähiges Repository. Für ein Testprojekt, das per `git clone` lokal ausgeführt wird, hätte ein Login keinen echten Schutzwert geboten (Zugangsdaten hätten ohnehin öffentlich im README stehen müssen) und stattdessen unnötige Komplexität hinzugefügt.
