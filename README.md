# UpGun Mod Tools Launcher — Avalonia

Portage du launcher WinForms (.NET Framework 4.8) vers **Avalonia UI 11** sur
**.NET 8**, multiplateforme (Windows / Linux / macOS), avec une UI sombre
redessinée.

## Ce qui a changé

- **UI entièrement redessinée** : chrome de fenêtre custom (titlebar dessinée
  à la main, plus de bordure Win32), palette sombre violet/turquoise, liste
  des mods en "cartes", tags en chips, zone de drag & drop pour le `.pak`.
- **Form1 → `Views/MainWindow.axaml`**, **Form2 → `Views/PublishWindow.axaml`**.
- `SteamWorkerTask` (le process worker headless `--worker-query` /
  `--worker-publish`) est **inchangé dans sa logique** — juste déplacé dans
  `Steam/SteamWorkerTask.cs` et légèrement adapté (l'appel P/Invoke
  `SetEnvironmentVariable` de kernel32, Windows-only, est remplacé par
  `Environment.SetEnvironmentVariable`, portable).
- Le chargement de la liste Workshop se fait maintenant en tâche de fond
  (`Task.Run`) au lieu de bloquer le thread UI comme dans le WinForms
  d'origine.
- `MessageBox.Show` n'existe pas en Avalonia : remplacé par une petite
  fenêtre `MessageDialog` maison, stylée pareil que le reste de l'appli.
- Les URL Discord s'ouvrent via un helper cross-plateforme (`UseShellExecute`,
  avec repli sur `xdg-open`/`open` si besoin).

## Ce qu'il te reste à faire

1. **Steamworks.NET** : copie `Libs/Steamworks.NET.dll` (le même que ton
   projet WinForms) + la lib native de chaque plateforme visée
   (`steam_api64.dll` / `libsteam_api.so` / `libsteam_api.dylib`) — voir
   `Libs/README.txt`.
2. **Icône** : dépose `ModLauncher.ico` dans `Assets/` — voir
   `Assets/README.txt`.
3. **Champ `m_nFileSize`** dans `Steam/SteamWorkerTask.cs` : j'ai ajouté la
   taille du fichier à la sortie de `--worker-query` en supposant le nom de
   champ standard de `SteamUGCDetails_t` dans Steamworks.NET. Si ta version
   du binding a un nom différent, adapte cette ligne.
4. Comme je n'ai pas accès à nuget.org depuis cet environnement, **je n'ai
   pas pu compiler/tester ce projet**. Fais un premier `dotnet restore` +
   `dotnet build` chez toi pour rattraper d'éventuelles coquilles de nommage
   (les noms de contrôles Avalonia évoluent parfois d'une version mineure à
   l'autre).

## Build / run

```bash
dotnet restore
dotnet run
```

## Publier pour une plateforme donnée

```bash
dotnet publish -c Release -r win-x64   --self-contained false
dotnet publish -c Release -r linux-x64 --self-contained false
dotnet publish -c Release -r osx-x64   --self-contained false
```

## Arborescence

```
UpGunModToolsLauncher.csproj
Program.cs                     # point d'entrée + routage CLI --worker-*
App.axaml / App.axaml.cs       # thème Fluent sombre
Steam/SteamWorkerTask.cs       # logique Steamworks (portable, quasi inchangée)
Models/WorkshopItem.cs
Views/
  MainWindow.axaml(.cs)        # ex-Form1
  PublishWindow.axaml(.cs)     # ex-Form2
  MessageDialog.axaml(.cs)     # remplace MessageBox.Show
Styles/Theme.axaml             # palette de couleurs + styles des contrôles
Assets/                        # icône d'appli
Libs/                          # Steamworks.NET.dll + libs natives
```
