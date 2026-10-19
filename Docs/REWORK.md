# Rework: from the Flash-style port to a standard Unity project

The first port (`Assets/Game` + `Assets/Starlite`) runs the original ActionScript logic on a Starling
emulation layer that builds every screen in code at runtime. This rework turns it into a conventional
Unity project you can edit in the Hierarchy and Inspector. The legacy port stays playable from
`Assets/Scenes/Main.unity` until every screen is converted, then it is deleted.

## Target layout

```
Assets/Rory/
  Art/Atlases/        sprite sheets (PNG), sliced into named Sprites from their Starling XML on import
  Art/Backgrounds/    full-screen backgrounds and story slides (Sprites)
  Art/Branding/       splash images
  Art/Effects/        particle textures
  Fonts/              TTF fonts + TextMeshPro font assets; Bitmap/ = Komikandy bitmap fonts (BitmapFontAsset)
  Audio/Music, SFX    AudioClips
  Data/               ScriptableObjects (levels, dishes, customers, level events, dialogue, desserts, hidden objects)
  Data/Source/        the original XML, re-importable via  Rory > Import Game Data
  Prefabs/            UI widgets, popups, match-3 tile, customer slot, ...
  Scenes/             Boot + one scene per screen
  Scripts/Runtime/    Rory.Runtime assembly (namespace Rory.*)
  Scripts/Editor/     Rory.Editor assembly: importers, scene/prefab builders, menu tools
  Tests/              Rory.Tests (PlayMode)
```

## Runtime architecture

| Original (AS3 / legacy port) | Native Unity |
|---|---|
| `Main`, `Game`, `Global`, `gameEngine` | `Boot` scene + `GameManager` (persistent), `SceneFlow` (scene loading with fade) |
| `Screen` subclasses, `fadeToScreen()` | one scene per screen with a `*Controller` MonoBehaviour; `SceneFlow.Go(GameScene.X)` |
| `TGlobalClass` statics | `GameSession` (current run state) on `GameManager` |
| `TSavegame`, `TPlayer`, `SharedObject` | `SaveSystem` + `PlayerProfile` (`[Serializable]`, JsonUtility) |
| `SoundAsset` / `SoundLib` | `AudioManager` (music source + SFX pool, `AudioLibrary` asset) |
| Starling display list built in code | scene hierarchies / prefabs (RectTransforms, `Image`, `TextMeshProUGUI`) |
| `TNonStateButton`, `TStringButton`, `ButtonWithSound` | `UnityEngine.UI.Button` + `RoryButton` (hover scale, click sound) |
| `TouchEvent` handlers | `Button.onClick`, `IPointer*Handler` |
| TweenLite / Juggler | `Rory.Tweening` (small tween runner, same eases) |
| PDParticleSystem | `UIParticleEmitter` (UI-space Particle Designer emitter) |
| XML via E4X | ScriptableObjects (`LevelDatabase`, `DishDatabase`, ...) |
| `AssetManager.getTexture("name")` | serialized `Sprite` references; `SpriteCatalog` for data-driven names |
| `TWindowDisplay` | `PopupManager` + popup prefabs |
| `Tutorial` | `TutorialManager` + dialogue data asset |

## Coordinates

All layouts keep the original 1024x600 design. Each scene has a `UIStage` (Canvas + CanvasScaler 1024x600,
letterboxed, clipped). Builders place elements with Starling coordinates (top-left origin, y down, pixel pivots)
converted to RectTransforms anchored at the stage's top-left, so positions in the Inspector read as
`(x, -y)` of the original code.

## Conversion order

1. Native assets: sprites, backgrounds, audio, fonts, data assets.
2. Core runtime: managers, saves, audio, scene flow, tweening, UI components, Boot scene.
3. Screens: Splash, MainMenu (+ player select), Opening/Ending, Credits, Achievements, LevelSelect,
   Kitchen (+ shops), Match3, HiddenObject, Dessert, popups, tutorial.
4. Switch the build to `Boot`, delete the legacy port, update tools and docs.

Each step keeps the project compiling; screens are verified with PlayMode tests and in the editor.
