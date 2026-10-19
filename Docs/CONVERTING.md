# Converting a screen (handbook)

The reference for behaviour and layout is the legacy C# port in `Assets/Game` (a line-by-line port of the
decompiled ActionScript; the raw AS3 is in `Reference/decompiled-as3`). The native version must look and play
the same, but be built from normal Unity parts. `MainMenu` (Scripts/Runtime/Screens/MainMenu,
Scripts/Editor/Builders/MainMenuBuilder.cs, Tests/MainMenuTests.cs) is the worked example: read it first.
Overall architecture: [REWORK.md](REWORK.md).

## What a converted screen consists of

| Part | Location | Notes |
|---|---|---|
| Controller | `Scripts/Runtime/Screens/<Screen>/<Screen>Controller.cs` | `: ScreenController`, namespace `Rory.Screens.<Screen>`. Serialized references to its scene objects, logic in plain methods/coroutines. |
| Helper behaviours | same folder | e.g. a customer slot, a tile, a shop page. One MonoBehaviour/ScriptableObject per file, file name = class name. |
| Builder | `Scripts/Editor/Builders/<Screen>Builder.cs` | static class with `[MenuItem("Rory/Build/Scenes/<Screen>")]` + `[RoryBuilder]` method that generates `Scenes/<Screen>.unity` with `UIBuild`. Scene name = the `GameScene` enum value. |
| Prefabs (optional) | `Prefabs/<Screen>/` | for things instantiated at runtime (tiles, customers). Build them in a `[RoryBuilder(50)]` method with `UIBuild.SavePrefab`. |
| Tests | `Tests/<Screen>Tests.cs` | PlayMode: load the scene, drive it, assert state, `TestUtil.Capture(...)` key moments. |

Scenes and prefabs are **generated**: change the builder, not the .unity file, then run the build.

## Layout rules (UIBuild)

* Coordinates are the original ones: x right, y **down**, origin top-left of the 1024x600 stage, pivots in
  pixels from the element's top-left. `UIBuild.Image(parent, name, sprite, x, y, pivotX, pivotY)` is
  `new Image(tex); pivotX/Y; x/y` in the original. `ImageCentered` = `alignPivot()` / pivot = size/2.
* Containers (Starling `Sprite`) are `UIBuild.Container(parent, name, x, y)`: zero size, children positioned
  relative to it, exactly like Starling children.
* **TNonStateButton quirk**: its x/y setters move the pivot to the centre but keep x/y as the top-left
  corner unless `alignPivot()`/`pivotX` was set first. So `btn.x = 880; btn.y = 5` without alignPivot means
  `UIBuild.Button(..., 880, 5, topLeft: true)`; with alignPivot or an explicit centre pivot use `topLeft: false`.
  Labels: `label:`, `font:`, `fontSize:`, `labelColor:` (TNonStateButton(texture, text, font, size, color)).
* `TStringButton` / clickable text: `UIBuild.TextButton` (grows 10% on hover).
* Text: `TextField(w, h, text, font, size, color)` becomes
  `UIBuild.Text(parent, name, text, font, size, color, x, y, w, h, pivotX, pivotY, align)`.
  Default alignment is centred like TextField; `hAlign = "left"` is `TextAlignmentOptions.Left`, use the `Top*`
  variants for `vAlign = "top"`. `autoScale = true` is `autoSize: true`. `bold` is `fontStyle = FontStyles.Bold`.
  Fonts: `UIBuild.Fonts.ByOriginalName("bromine" | "jenna_sue" | "pfennig" | "komikandy" | "KomikandyGold" | ...)`.
  Colours: `UIBuild.Hex(0xRRGGBB)` (the original passes decimal ints, e.g. 3875592).
* Rotation: Starling radians clockwise become Unity degrees counter-clockwise: `localEulerAngles.z = -deg`
  (`StageLayout.SetStarlingRotation` at runtime). Scale is the same.
* Sprites: `UIBuild.Sprite("<atlas>", "<name>")` (folders under Art/Sprites are the original atlas names),
  `UIBuild.Background("kitchen_BG")`, `UIBuild.Frames("title", "asap_")` for MovieClip frame lists.
  At runtime, names that come from data go through `GameDatabase.Instance.sprites.Get("<atlas>", name)`.
* Moving things at runtime: `rt.SetStagePosition(x, y)`, and `StageLayout.ToAnchored(x, y)` for tween targets.
* Draw order = sibling order (the original's addChild order). Keep it.
* Hidden-object style pixel-precise clicks: `image.alphaHitTestMinimumThreshold = 0.5f` (HO sprites are readable).
* Layers of every scene (from `UIBuild.NewScene`): your content under the stage, then `Popups`
  (PopupManager), then `Overlay` (`UIBuild.Overlay(stage)`: tutorial bubble, achievement banner).

## Runtime rules

* State: `Player` (PlayerProfile, saved), `Session` (GameSession, not saved), `Db` (GameDatabase: levels,
  dishes, customers, levelEvents, dialogues, desserts, hiddenObjectLayouts, sprites, audio), `Sound`
  (AudioManager), `GoTo(GameScene.X)` (fade + load). Save with `Game.Saves.Save()` where the original called
  `TSavegame.save()`. Money is a plain int (`maxCrypto.STI/ITS` disappear).
  `TGlobalClass.X` becomes `Session.x` / `GameRules.X`; `TSavegame.currentPlayer` becomes `Player`.
  Port index arithmetic (days, dishes, slots) exactly as written; don't "fix" off-by-ones silently. If you find
  a real bug in the original, keep the original behaviour and mention it in your report.
* **Never write to ScriptableObject data at runtime** (it persists in the editor). Copy values first.
* The original ran at **30 fps** with per-frame logic (`Update()` called once per frame). Convert per-frame
  counters and speeds to time: `x += speed` per frame becomes `x += speed * 30 * Time.deltaTime`; a counter
  of N frames becomes `N / 30f` seconds. Keep gameplay timing identical.
* TweenLite becomes `Rory.Tweening`:
  `TweenLite.to(o, d, {x, y, ease, delay, onComplete})` is
  `rt.TweenAnchoredPosition(StageLayout.ToAnchored(x, y), d).SetEase(Ease.BackOut).SetDelay(t).OnComplete(...)`;
  `alpha` is `TweenAlpha` (Graphic or CanvasGroup); `scaleX/Y` is `TweenScale`; `rotation` is `TweenRotation(-deg)`;
  anything else is `Tweens.Float(owner, from, to, d, setter)` or `Tweens.To(owner, d, p => ...)`.
  `TweenLite.delayedCall(t, f)` is `Tweens.DelayedCall(t, f, this)`; `killTweensOf(o)` is `o.KillTweens()`;
  `TimelineLite` is `Tweens.Sequence().Append(...)`. TweenLite's default ease is `Ease.QuadOut` (also ours).
  Eases: `Back.easeOut` is `Ease.BackOut`, `Strong.*` is `Ease.Strong*`, `Linear.easeNone` is `Ease.Linear`, etc.
* `MovieClip` + juggler: `SpriteAnimator` (`UIBuild.Animation`). `PDParticleSystem`: `UIParticleEmitter`
  (`UIBuild.Particles(parent, name, config, x, y)`; configs are `Art/Effects/*.asset`).
* Touch: `TouchEvent` BEGAN/ENDED on buttons becomes `Button.onClick`; drags and press-to-act use
  `IPointerDownHandler`/`IDragHandler`/`IPointerUpHandler` and `UIStage.Current.ScreenToStage(e.position)`
  to get original coordinates. `touchable = false` is `raycastTarget = false` (UIBuild images default to false).
* Sounds: `Sound.PlaySfx("name")` (file name without .mp3), `Sound.PlayMusic("track")`; `musicPlayingIdx`
  checks disappear (PlayMusic ignores the track already playing; `Sound.IsPlayingMusic(a, b)` for the random pair).
* Windows: shared windows are prefabs in GameDatabase (`Windows.Big()/Small()/Confirm()/Message()`);
  screen-only windows live in the scene as inactive popups (the `ScenePopup` pattern in MainMenuBuilder) and open
  with `PopupManager.Current.ShowExisting(popup)`. The 70% black screen is the PopupManager backdrop.
* Achievements: `Achievements.Add(Player, id, amount, progressEvery)` then
  `AchievementNotifier.Current?.Show(id, result)`; scenes that notify call `AchievementBuilder.AddNotifier(stage)`.
* Dropped features (leave them out): ads, GALib analytics, "more games", free coins / video ads, full-version
  unlock, leaderboards, cross promo, lost-context handling, asset loading queues (assets are referenced directly).

## Building and testing (batch mode, no editor window)

Unity: `/c/Program Files/Unity 2023.2.22f1/Editor/Unity.exe` (never use any other Unity install).
Only one Unity process may open a project folder at a time. From your project folder:

```bash
U="/c/Program Files/Unity 2023.2.22f1/Editor/Unity.exe"
# compile + regenerate all prefabs and scenes
"$U" -batchmode -nographics -projectPath "$PWD" -executeMethod Rory.EditorTools.BuildAll.BatchBuild -logFile "$PWD/Logs/build.log"
grep -n "error CS\|Exception" Logs/build.log
# PlayMode tests (no -nographics: captures need rendering)
"$U" -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/Logs/tests.xml" -logFile "$PWD/Logs/tests.log"
grep -o 'total="[0-9]*" passed="[0-9]*" failed="[0-9]*"' Logs/tests.xml
# only if you change importers or data: -executeMethod Rory.EditorTools.ProjectSetup.BatchSetup
```

A first-pass `CS0246` followed by "Tundra requires additional run" in the log is harmless when a second
compile follows. `-testFilter <regex>` runs a subset. Captures land in `Logs/Captures/<name>.png`; look at them.

**Reference screenshots of the original**, rendered headlessly from the legacy port:

```bash
H="/c/Program Files (x86)/GameTop.com/Rory's Restaurant Deluxe/modkit/unity_stubs"
RES="$H/out_run/project/Assets/Resources"
OUTRUN=/d/RoryWork/harness-<you> bash "$H/run_headless.sh" my_script.txt "$RES" /d/RoryWork/ref-<you>
```

Script commands, one per line: `sec N`, `wait FRAMES`, `click X Y`, `drag X1 Y1 X2 Y2`, `clicktext Label`,
`tryclicktext Label`, `type text`, `day N` (sets passedDays/currentPlayedLevel/region),
`goto kitchen|match3|ho|dessert|levelselect|menu|achievement|ending|opening`, `autoswap` (one match-3 move),
`hofind` (find one hidden object), `shot name`, `tree DEPTH` (print the display list with positions).
Start scripts with: `sec 12`, `clicktext Story`, `sec 2`, `type Tester`, `clicktext Done`, `sec 3`,
`clicktext Select`, `sec 2`. Always use your own `OUTRUN` folder. The renderer is approximate (GDI) but
positions and sizes are exact; `tree` gives precise coordinates.

## Ground rules for parallel work

* Work only in your own project folder and branch; commit there (messages end with
  `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`); do not push, merge or rebase.
* Own only the files of your assignment. Shared files you may touch, additively only: `GameSession.cs`
  (new fields in a block commented with your area), `GameRules.cs` (new constants), `TestUtil.cs`
  (new helpers). Anything else shared (UIBuild, managers, popups base, data model): don't edit; work around
  it in your own code and list what you needed in your report.
* Don't commit `Logs/`, `Library/`, `UserSettings/`. Do commit generated scenes/prefabs with their .meta files,
  and `ProjectSettings/EditorBuildSettings.asset` as the build writes it.
* Never run the legacy scene or modify `Assets/Game`, `Assets/Starlite`, `Assets/Resources/RR`.
