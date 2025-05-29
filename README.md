# MP133 – ASP.NET Webanwendung mit Azure-Integration

## 📌 Projektbeschreibung

MP133 ist eine moderne ASP.NET Core Webanwendung, die auf Microsoft Azure gehostet wird. Die Anwendung verwendet eine Azure SQL-Datenbank zur Datenhaltung und wurde mit dem Ziel entwickelt, eine benutzerfreundliche Plattform zur Verwaltung von Mitarbeitendeninformationen bereitzustellen.

---

## ⚙️ Verwendete Technologien

- **ASP.NET Core 8.0** – Webentwicklung mit Razor Pages
- **C#** – Programmiersprache
- **Entity Framework Core** – Datenbankzugriffe und Migrationen
- **Azure SQL-Datenbank** – Cloudbasierte Datenhaltung
- **Azure App Services (Web Deploy)** – Bereitstellung in der Azure-Cloud
- **ReCaptcha** – Schutz gegen Bots
- **.NET CLI Tools** – Kommandozeilen-Tools zur Verwaltung und Entwicklung

---

## 🚀 Setup-Anleitung

### Voraussetzungen

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/)
- Azure-Konto mit App Service & SQL-Datenbank

### Lokale Installation

1. Repository klonen:

   ```bash
   git clone https://github.com/dein-benutzername/MP133.git
   cd MP133
   ```

2. Datenbankverbindung konfigurieren in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
   }
   ```

3. Migrationen anwenden:

   ```bash
   dotnet ef database update
   ```

4. Anwendung starten:

   ```bash
   dotnet run
   ```

---

## ☁️ Deployment auf Azure

Die Bereitstellung erfolgt über Azure App Services mit dem enthaltenen `Web Deploy` Profil.

1. In Visual Studio:
   - Rechtsklick auf das Projekt > **Veröffentlichen**
   - **App Service (Windows)** auswählen
   - Einstellungen aus `profile.arm.json` übernehmen

2. Azure SQL-Datenbank muss separat eingerichtet werden (Konfiguration siehe `appsettings.json`).

---

## 🗃️ Datenbankmodell

Das Projekt verwendet Entity Framework Core mit Code-First-Migrationen. Siehe Ordner `Migrations/` und `Data/MP133Context.cs`.

---

## ✨ Features

- Benutzerverwaltung (CRUD)
- Azure SQL-Integration
- ReCaptcha-Integration
- Deployment via Web Deploy
- Responsives Razor Page Layout
- E-Mail-Benachrichtigung via `EmailService.cs`

---

## 📄 Lizenz

Dieses Projekt ist unter keiner spezifischen Lizenz veröffentlicht. Bitte kontaktiere den Autor bei Interesse zur weiteren Verwendung.

---

## 🧑‍💻 Autor

- **Dein Name oder Teamname**
- Kontakt: [Deine E-Mail oder Website]
