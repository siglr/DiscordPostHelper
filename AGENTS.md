# AGENTS.md

[PROJECT]
WeSimGlide.org Integration Suite — multi-component .NET + PHP solution for automating soaring task handling, Discord posting, and MSFS integration.
Goal: unified ecosystem for creating, posting, loading, and analyzing virtual soaring events.

---

[TECH_STACK]

* **Solution language mix:**

  * C# (.NET Framework 4.8) — main WinForms and WPF apps
  * VB.NET (.NET Framework 4.8) — updater console app
  * PHP 8.x — server-side scripts
* **Shared libraries:** CommonLibrary (C#, .NET 4.8)
* **Database:** SQLite (server side)
* **Environment assumption:** Windows 10/11 desktop environment with MSFS SDK-related paths.
* **Important rule:**
  ✅ *Always read `.sln` and `.csproj` / `.vbproj` files before generating or modifying code.*

  * Match target framework versions and language type (C# vs VB).
  * Preserve project references and post-build events.
  * Use existing `CommonLibrary` namespaces and conventions.
  * Maintain x64 build configuration (since 2025 migration).
* **Forbidden:**

  * Mixing .NET 6+ syntax or nullable annotations unless project explicitly migrated.
  * Generating code outside defined namespaces.
  * Changing framework version or SDK type without explicit upgrade instruction.

---

[OBJECTIVES]

* Automate event and task posting to Discord.
* Unpack and load soaring task packages into MSFS automatically.
* Maintain synchronized communication with WeSimGlide.org backend.
* Enable local HTTP messaging between apps.
* Support bilingual static message content (EN/FR) in Discord posts.
* Centralize logging, versioning, and configuration logic.

---

[CORE_COMPONENTS]

### DPHX Unpack & Load

* Type: WinForms (.NET 4.8.1, VB).
* Role: unpack `.DPHX` archives, extract `.PLN`, `.WPR`, and other
* Communicates with: WSGListener (local HTTP).
* Calls: PHP endpoints (remote).
* Integrates shared CommonLibrary utilities.
* Target: x64 build.
* Goal: “Click-to-Fly” workflow.

### Discord Post Helper (DPH)

* Type: WinForms (.NET 4.8.1, VB).
* Builds structured Discord posts for group events.
* Localizes static content (EN/FR) but leaves dynamic input untranslated.

### WSGListener

* Type: VB background service (.NET 4.8.1).
* Role: local HTTP API bridge for DPHX, called by WeSimGlide.org.

### CommonLibrary

* Type: Class Library (.NET 4.8.1, VB).
* Provides shared methods for logging, config, HTTP, JSON, XML, and version management.
* Required dependency for all major apps.
* Central point for constants and models.

### ImageViewer

* Type: WinForms utility (.NET 4.8.1, C#).
* Non-critical.

### Updater

* Type: VB.NET Console App (.NET 4.8.1, VB).
* Handles updates for local tools via server version metadata.
* Performs safe file replacement and logging.
* Typically triggered by DPHX and DPH.

### AutoIGCParserBatch

* Type: C# internal tool.
* Batch processes IGC files for upload via PHP endpoints.
* Admin-only use.

---

[SERVER_COMPONENTS]

* Language: PHP 8.x + SQLite 3.7.17.
* Shared file: `CommonFunctions.php` (logging, config, permissions).
* Used by: DPHX, DPH, AutoIGCParserBatch.
* Logs and handles task, event, and user operations.

---

[LEGACY_COMPONENTS]

* **TaskBrowserAdmin:** old WinForms admin tool — deprecated.
* **WindLayers3DDisplay:** WPF experimental app for weather visualization — deprecated.
* Keep for reference only

---

[RELATIONSHIPS]

```
DPHX Unpack & Load
 ├─ uses → CommonLibrary
 ├─ uses → ImageViewer
 ├─ communicates → WSGListener (HTTP)
 ├─ shares a few source code files with AutoIGCParserBatch
 └─ triggers → Updater

Discord Post Helper
 ├─ uses → CommonLibrary
 ├─ uses → ImageViewer
 ├─ calls → PHP backend
 └─ posts → Discord webhooks

WSGListener
 ├─ receives → requests from DPHX and WeSimGlide.org
 └─ relays → data to DPHX

AutoIGCParserBatch
 ├─ shares a few source code files with DPHX Unpack & Load
 └─ uploads → IGCs via PHP endpoints

CommonLibrary
 └─ shared dependency → all apps
```

---

[FUTURE_WORK]

* Deprecate legacy visualization tools.
* Document and expand the BriefingControl render context API (host mode, installed sims, preset name display) as new host integrations are added.
* Document the club timing message fields (MeetMessage, NoSyncMessage, SyncMessage, LaunchMessage, StartMessage) in the RetrieveSoaringClubs API contract.

---

[TEST_CRITERIA]

* Local HTTP comms verified (listener active, correct port).
* PHP API returns valid JSON.
* Discord posts correct in EN and FR with proper date/time formatting.
* Update cycle completes without corruption.
* Missing translations fall back gracefully.

---

[CODEX_RULES]

1. Always inspect `*.sln` and project configuration before any code change.
2. Respect the target `.NET Framework` version and `x64` build setting.
3. Use consistent namespaces as defined in each project.
4. Treat PHP scripts as immutable server API endpoints unless modification is explicitly requested.
5. For refactors: maintain backward compatibility with all integrated components.
6. For new features: document API additions in this file under `[FUTURE_WORK]`.

---
