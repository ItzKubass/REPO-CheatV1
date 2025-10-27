# 🎯 REPO Debug Overlay — Unity Visualizer (for Developers)

<div align="center">

![Unity](https://img.shields.io/badge/Unity-2022.3+-black.svg?style=for-the-badge&logo=unity)  
![.NET](https://img.shields.io/badge/.NET-Standard%202.1-512BD4.svg?style=for-the-badge&logo=dotnet)  
![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)  
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6.svg?style=for-the-badge&logo=windows)

*A developer-focused Unity overlay for visual debugging and runtime inspection of game objects.*

**ℹ️ Intended use:** This project is intended **only** as a developer tool for debugging, testing and visualization in Unity. Do **not** use it to modify multiplayer games in ways that violate their Terms of Service.  

</div>

## 📖 Table of Contents
- [Overview](#-overview)
- [✨ Features](#-features)
- [🛠️ Technical Details](#️-technical-details)
- [📁 Project Structure](#-project-structure)
- [🚀 Installation](#-installation)
- [⚙️ Configuration](#️-configuration)
- [🔧 Code Examples](#-code-examples)
- [📜 License](#-license)
- [❓ FAQ](#-faq)
- [👨‍💻 Credits](#-credits)

## 🎮 Overview

This is a Unity-based **debug overlay** that provides real-time visual overlays for development: object highlighting, distance markers, health/HP debug bars, item markers, and other runtime information rendered on the screen for testing and QA.

> **Note:** Use this tool for development, testing and debugging only. Respect game policies and do not use visualization tools to gain unfair advantage in multiplayer environments.

## ✨ Features

| Feature | Description | Status |
|---------|-------------|--------|
| 🎯 **Entity Highlighting** | Visual highlighting of selected entities with labels | ✅ Working |
| 📏 **Distance Display** | Real-time distance calculation to tracked objects | ✅ Working |
| ❤️ **Health Display** | Health points / status overlay for debug builds | ✅ Working |
| 💎 **Item Markers** | Mark and label important in-game objects (e.g. pickups) | ✅ Working |
| 🔄 **Cross-scene** | Optional persistence across scene loads (dev builds) | ✅ Working |
| 🎨 **Custom GUI** | Simple, customizable overlay using IMGUI or Unity UI | ✅ Working |

## 🛠️ Technical Details

### Requirements
- **Unity**: 2022.3 or newer (LTS recommended)  
- **.NET**: Standard 2.1 compatible runtime for editor/runtime scripts  
- **Intended build target**: Development builds or Editor only (avoid shipping debug overlays in release builds)  
- **Dependencies** (optional, depending on your project):  
  - `Assembly-CSharp.dll` (project classes)  
  - `UnityEngine.CoreModule.dll`  
  - `UnityEngine.IMGUIModule.dll`  

### Architecture
```csharp
namespace REPO.DebugOverlay
{
    public class Loader    // Initializes the overlay (attach in scene or load via [RuntimeInitializeOnLoadMethod])
    public class Hacks     // Main functionality: finds objects, caches, toggles
    public class Render    // Drawing utilities: WorldToScreen, draw labels, boxes, bars
}
```

## 📁 Project Structure

```
REPO-DebugOverlay/
├── 📄 Hacks.cs           # Main debug logic & GUI toggles
├── 📄 Loader.cs          # Runtime initialization & persistence helpers
├── 📄 Render.cs          # Rendering utilities (WorldToScreen, draw primitives)
├── 📄 REPO.csproj        # Project configuration (if used as separate assembly)
├── 📄 LICENSE.txt        # MIT License
└── 📄 README.md          # This file
```

## 🚀 Installation

**Method 1 — Install into your Unity project (recommended for dev use)**

1. Clone or download the repo:
```bash
git clone https://github.com/ItzKubass/REPO-DebugOverlay.git
cd REPO-DebugOverlay
```

2. Copy the `Hacks.cs`, `Loader.cs`, `Render.cs` (or the `Runtime` folder) into your Unity project's `Assets/` folder.

3. In Unity Editor, open the scene you want to debug and add the `Loader` GameObject (or attach `Loader` to an existing persistent GameObject). Enable "Development Build" in Build Settings if you want runtime usage outside editor.

**Method 2 — Use as a compiled assembly (optional)**
- Build the project as a DLL targeting .NET Standard 2.1 and drop the produced DLL into `Assets/Plugins/`. Then attach the `Loader` or use the `[RuntimeInitializeOnLoadMethod]` to auto-register.

> Do **not** distribute debug overlays in production release builds. Keep them gated behind development symbols or Editor-only checks.

## ⚙️ Configuration

Modify runtime settings directly in `Hacks.cs` or expose them via a ScriptableObject / Editor window:

```csharp
public class Hacks : MonoBehaviour
{
    public bool entityHighlight = true;         // Toggle entity highlighting
    public bool showDistance = true;            // Show distance labels
    public bool showHealthBars = true;          // Display health/status bars (if available)
    public float cacheInterval = 1f;            // Update frequency for object caching
}
```

Consider wrapping these in `#if UNITY_EDITOR` or `Debug.isDebugBuild` checks to ensure they are only active when intended.

## 🔧 Code Examples

### Main update / caching loop
```csharp
void Update()
{
    // Cache objects every `cacheInterval` seconds for performance
    if (Time.time - lastCacheTime > cacheInterval)
    {
        cachedValuables = Object.FindObjectsOfType<ValuableObject>(); // project-specific
        cachedEnemies = Object.FindObjectsOfType<Enemy>();           // project-specific
        lastCacheTime = Time.time;
    }

    // Render overlays
    foreach (var e in cachedEnemies)
    {
        Vector3 screenPos = WorldToScreen(e.transform.position);
        if (screenPos.z > 0) // in front of camera
        {
            Render.DrawLabel(screenPos, e.name);
            if (showHealthBars)
                Render.DrawHealthBar(screenPos, e.Health / e.MaxHealth);
        }
    }
}
```

### World-to-Screen conversion (safe, general-purpose)
```csharp
Vector3 WorldToScreen(Vector3 worldPosition)
{
    // Use camera to convert world position to screen coordinates
    Camera mainCam = Camera.main;
    if (mainCam == null) return Vector3.zero;

    Vector3 pos = mainCam.WorldToScreenPoint(worldPosition);
    // Unity's WorldToScreenPoint already returns screen coords where z > 0 means in front
    return pos;
}
```

> Tip: prefer `Camera.WorldToScreenPoint` for clarity and correctness; the manual matrix multiplication variant is shown below for educational purposes.

Manual matrix example (educational):
```csharp
Vector3 WorldToScreenManual(Vector3 worldPosition)
{
    Matrix4x4 matrix = mainCam.projectionMatrix * mainCam.worldToCameraMatrix;
    Vector4 pos = matrix * new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 1f);

    float x = (pos.x / pos.w + 1f) * 0.5f * Screen.width;
    float y = (1f - pos.y / pos.w) * 0.5f * Screen.height;

    return new Vector3(x, y, pos.z);
}
```

## 📜 License

This project is licensed under the MIT License. See `LICENSE.txt` for details.

```
MIT License
Copyright (c) 2024 ItzKubass
```

## ❓ FAQ

**❔ Is this detected?**  
This is a local developer tool meant for testing and debugging. Do not use it in ways that break a game's Terms of Service.

**❔ Can I get banned?**  
If used to alter or automate behavior in online games, yes—modifying live multiplayer clients can result in bans. Keep the tool for local development or authorized QA only.

**❔ How often is this updated?**  
Updates are released as needed for compatibility improvements and additional debug features.

**❔ Can I contribute?**  
Yes — please submit pull requests or open issues that target legitimate development and debugging uses.

## 👨‍💻 Credits

Developed by ItzKubass

<div align="center">
⭐ Don't forget to star this repository if you find it useful!

Last updated: December 2024
</div>
