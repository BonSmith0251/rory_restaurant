# Porting guide: Rory's Restaurant AS3 → Unity C# (Starlite)

The original game is ActionScript 3 on Starling 1.x. Its decompiled source is in
`modkit/decompiled/*.as` (pseudo-AS3: control flow is `goto`/labels, stack temporaries are `$s0`, `$t0`).
Starlite (`Assets/Starlite/*.cs`) re-implements the Starling/Flash/TweenLite APIs the game uses,
so the port is close to line-by-line. **Goal: identical behaviour, same names, readable structured C#.**

## Files and naming
- One C# file per AS3 class: `Assets/Game/<ClassName>.cs`. Drop packages: `rudi.Global` → `Global`,
  `CubicHermiteInteropolation.BezierPath` → `BezierPath`. Everything lives in the global namespace.
- Keep **every class, field, method, property and constant name exactly as in AS3** (case included).
  Other classes are ported in parallel and reference your members by their AS3 names.
- Make everything `public` (fields, methods, properties, statics). Don't make classes `sealed`/`static`
  unless the AS3 class only has statics (static-only AS3 classes: still a normal `public class` with static members).
- Methods overridden in subclasses: base `public virtual`, subclass `public override`. AS3 `override` ⇒ C# `override`.
- If a member name equals a C# keyword use `@name`. If a member name equals its class name, append `_` and note it in your report.
- Usings for game files (and nothing else from Unity):
  ```csharp
  using System;
  using System.Collections.Generic;
  using Starlite;
  using Mathf = UnityEngine.Mathf;   // only if you need it
  ```
  **Never** `using UnityEngine;` (it clashes with Sprite/Image/Texture/Color/Random/Event). Don't use `MathF`.

## Type mapping (apply to fields, params, return types; must be consistent across classes)
| AS3 | C# |
|---|---|
| `int`, `uint` | `int` (uint → int everywhere) |
| `Number` | `float` (AS3 default NaN: if code checks `isNaN`, initialize to `float.NaN`) |
| `Boolean` | `bool` |
| `String` | `string` |
| `Vector.<T>` | `List<T>` (mapped element type, nested ok: `List<List<int>>`) |
| `Array` | `List<object>` unless every use is clearly one type, then `List<T>` (prefer precise if the field is only used inside your class) |
| `Object` used as map / literal | `Dictionary<string, object>`; otherwise `object` |
| `*` | `object` |
| `Function` | `Delegate` (call with `AS3.Invoke(fn, args)`) |
| `Class` | `Type` |
| `XML` / `XMLList` | `XML` / `XMLList` (Starlite) |
| Starling/Flash types | same name in Starlite (Sprite, Image, Quad, TextField, MovieClip, Texture, TextureAtlas, AssetManager, Juggler, Tween, Event, TouchEvent, Touch, TouchPhase, KeyboardEvent, Point, Rectangle, Sound, SoundChannel, SoundTransform, SharedObject, BitmapFont, PDParticleSystem, BitmapData, Bitmap, PixelHitArea, PixelImageTouch, BlurFilter, TweenLite, TimelineLite, ...) |
| `flash.text.TextField` (native input) | `NativeTextField` (Starlite), add to `_global._stage` as before; drop the stage-scale factors (everything is 1024×600) |

## Statements and semantics
- Rewrite `goto` spaghetti into structured `if/else/while/for/switch/break/continue/return`, preserving exact
  semantics (order of side effects, early returns). Typical loop: `goto Lc; Lb: …; Lc: if (cond) goto Lb;` ⇒ `while (cond) { … }`.
  `$sN`/`$tN` temporaries come from `&&`, `||`, `?:` and assignment chains: rebuild the original expression.
  `switch(x){case 0: goto…}` blocks preceded by `if (1 !== v) goto…` chains are an AS3 `switch (v)`: rebuild it.
  `hasnext2/nextname/nextvalue` = `for (k in obj)` / `for each (v in obj)` ⇒ `foreach`.
- **Division**: AS3 `/` is always floating point. If both C# operands are ints, cast: `(float)a / b`.
  `int(x)` ⇒ `(int)x` for floats (truncation), `AS3.toInt(x)` for strings/objects. `Math.floor(x)` ⇒ `Mathf.Floor(x)` (float) –
  cast with `(int)` where an int is needed. `Math.random()` ⇒ `AS3.random()`. `Math.max/min/abs/pow/sqrt/sin/cos/atan2/PI` ⇒ `Mathf.*`.
- `%` on ints is the same. Integer literals like `4294967046` are negative ints wrapped by the AS3 compiler: use `-250` (= value − 2^32).
- Strings: `.length` ⇒ `.Length`; `String(x)` ⇒ `AS3.str(x)`; `Number(s)` ⇒ `AS3.toNumber(s)`; `parseInt` ⇒ `AS3.parseInt`.
  AS3 methods exist as extensions: `substr`, `substring`, `slice`, `charAt`, `charCodeAt`, `indexOf`, `split` (returns `List<string>`),
  `toUpperCase`, `toLowerCase`, `replace` (first occurrence, like AS3 with a string pattern).
  **Concatenating a float into a string must use `AS3.str(f)`** (C# float formatting is culture dependent). ints are fine.
- Arrays/Vectors (`List<T>`): `.length` ⇒ `.Count`; `length = 0` ⇒ `.Clear()`; `length = n` ⇒ `.setLength(n)`;
  `push/pop/shift/unshift/splice/slice/join/indexOf/concat` are extensions. `new Vector.<T>(n, true)` ⇒ `AS3Extensions.filled<T>(n, i => default)`.
  AS3 reading past the end returns undefined instead of throwing – use `.at(i)` where the code relies on that; writing past the end grows the array – use `.set(i, v)`.
- `trace(...)` ⇒ `AS3.trace(...)`; `getTimer()` ⇒ `AS3.getTimer()`; `isNaN` ⇒ `float.IsNaN`.
- `x is T` / `x as T` are the same in C#. AS3 truthiness (`if (obj)`, `if (str)`) ⇒ explicit `!= null`, `!string.IsNullOrEmpty`, `!= 0`.
- Closures (`function(){…}` inside a method) ⇒ C# lambdas; captured `$activation` slot variables are just captured locals.
- `try/catch` around LocalConnection GC tricks, `System.gc()`, `System.pauseForGCIfCollectionImminent()`, `Font.registerFont`,
  `Starling.current.showStats`, `Context3D` stuff ⇒ delete.

## Events and input
- Listener methods take the base type: `public void touchButton(Event arg1) { var e = (TouchEvent)arg1; … }`
  (also for KeyboardEvent/EnterFrameEvent). Methods with an optional event param `(arg1:Event = null)` ⇒ `(Event arg1 = null)`.
  Register with method groups: `btn.addEventListener(TouchEvent.TOUCH, this.touchButton);` remove the same way.
  Zero-argument listeners are also allowed (`Action`).
- Touch API is Starling's: `e.getTouch(this, TouchPhase.BEGAN)`, `touch.getLocation(target)`, `touch.globalX`, `e.currentTarget`.
- `dispatchEvent(new TutorialEvent(...))`, `dispatchEventWith(Event.TRIGGERED)` work as in Starling (bubbling supported).

## Tweens and timing
```csharp
TweenLite.to(obj, 0.5f, new Vars { ["x"] = 100, ["scaleX"] = 1.2f, ["ease"] = Expo.easeOut,
    ["onComplete"] = (Action)this.onDone });
TweenLite.to(obj, 0.5f, new Vars { ["y"] = 10, ["onComplete"] = (Action<int>)this.onDoneIdx, ["onCompleteParams"] = new object[] { idx } });
TweenLite.to(obj, 1, new Vars { ["alpha"] = 0, ["onComplete"] = (Action)(() => { obj.visible = false; }) });
TweenLite.delayedCall(2, (Action)this.next);               // or delayedCall(2, (Action<int>)f, new object[]{3})
TweenLite.killTweensOf(obj);  TweenLite.killDelayedCallsTo((Action)this.next);
var tl = new TimelineLite(); tl.to(target, 0.8f, new Vars{...}, 0); tl.append(new TweenLite(...)); tl.vars["onComplete"] = (Action)f;
Starling.juggler.add(movieClip); Starling.juggler.delayCall((Action)f, 1.5f);
```
Keys are the AS3 property names. Eases: `Linear.easeNone`, `Expo.easeIn/Out/InOut`, `Back.*`, `Bounce.*`, `Elastic.*`, `Quint.*`.
Tweens can target any object's public float/int field or property by name (reflection) – keep fields public.
For `killDelayedCallsTo` to work, schedule with the same delegate shape, e.g. `(Action)this.next` both times.

## Display, assets, text
- Display API = Starling 1.x: `addChild/addChildAt/removeChild/removeFromParent(dispose)/setChildIndex/getChildIndex/numChildren/getChildAt`,
  `x y pivotX pivotY scaleX scaleY rotation(radians) alpha visible touchable width height bounds alignPivot() getBounds(space)
  localToGlobal globalToLocal hitTest clipRect blendMode useHandCursor filter`. `Image(texture)`, `image.texture = t; image.readjustSize()`,
  `Quad(w, h, color)`, `MovieClip(List<Texture>, fps)` (+ `Starling.juggler.add(mc)`), `Sprite`.
- Assets exactly as the game did: `_global.assetManager.enqueue(_global.graphicsPath + "kitchen.ean", …)`,
  `loadQueue((float ratio) => { if (ratio == 1) initialize(); })`, `getTexture(name)`, `getTextureAtlas(name)`,
  `atlas.getTexture(name)`, `atlas.getTextures("asap_")`, `getXml(name)`, `getBitmapData(name)`, `purge()`.
- Text: `new TextField(w, h, text, fontName, fontSize, color, bold)` / `ExtendTextField(...)` with `hAlign`, `vAlign`, `autoScale`, `color`, `text`.
  Font names stay as in AS3 ("bromine", "jenna_sue", "Pfennig", "komikandy"; bitmap fonts "KomikandyGold", "KomikandyWhite", "KomikandyLarge").
- `[Embed]` classes: `new KomikandyGold()` (bitmap) ⇒ `new Bitmap(AS3.embeddedTexture("MainMenuScreen_KomikandyGold"))`,
  `Texture.fromBitmap(new X())` ⇒ `AS3.embeddedTexture("<Owner>_<X>")`; XML/ByteArray embeds ⇒ `AS3.embeddedXml("<Owner>_<X>")`.
  Symbols: MainMenuScreen_KomikandyGold/White/Large (+XML), MainMenuScreen_CongratsConfig, MainMenuScreen_CongratsParticle,
  MiniGameHO_FireConfig, MiniGameHO_FireParticle, Settings_settingXm.
- E4X: `xml.day[i].gameType` ⇒ `xml["day"][i]["gameType"]`; `.@attr` / `.attribute("attr")` ⇒ `["@attr"]` / `.attribute("attr")`;
  `.length()` same; `.children()[0]` same; string conversion is implicit (`string s = xml["x"];`), numbers via `.toInt()` / `.toFloat()`,
  `new XML(text)` same. Empty lookups return empty lists (never null) like E4X; indexing past the end returns null.
- Saves: `SharedObject.getLocal(name)`, `so.data["key"]` (Dictionary), `so.flush()`.
- Sounds go through the game's `_global._soundAsset` (ported centrally): keep the calls (`playSFX`, `playMusic`, …).
- URLs: `navigateToURL(new URLRequest(u))` ⇒ `Net.navigateToURL(new URLRequest(u))`; `NativeApplication.nativeApplication.exit()` stays.
- Analytics/ads: keep `GALib.TrackEvent(...)` / `AdsClass.*` calls – they are no-op stubs.

## Ownership (do not write classes owned by someone else; reference them by AS3 names)
| Owner | Classes |
|---|---|
| core | Game, Main (→ GameBootstrap), Global, gameEngine, Screen, TGlobalClass, ConstClass, SoundAsset (+rudi.sound.*), maxCrypto, assetPath, cSprite, ButtonWithSound, TNonStateButton, TStringButton, ExtendTextField, GALib, AdsClass, Dish |
| A | GameScreen, Match3Object, FrozenTile, BezierPath, CharLocation, TextureInfo, cQuad |
| B | TWindowDisplay, TWindow, TWindowState |
| C | Kitchen, TCustomer |
| D | MiniGameMakeDessert, MiniGameHO, HOClass, HOObjects, PixelPerfectObject |
| E | Tutorial, TutorialEvent, MainMenuScreen, LevelSelectScreen, Credits |
| F | OpeningScreen, EndingClass, AchievementScreen, Achievement, dumLoading, TSavegame, TPlayer, Settings, ScoreManager |

Core API you can rely on (same as the AS3): `Global.getInstance()`, `_global.assetManager`, `_global.unPurgedAsset`, `_global._game.fadeToScreen(screen)`,
`_global._stage` (Starlite `Stage`), `_global._soundAsset`, `_global._maxCrypto.STI/ITS/RandomRange`, `Screen` base with
`public virtual` `Start/Kill/Update/Pause/Resume/Suspend/PreStart/PostFadeIn/PostFadeOut/OnKeyDown(int)/OnKeyHit(int)/…` and
fields `name, ready, autoFadeIn…`. `TGlobalClass` statics. `ButtonWithSound`/`TNonStateButton` as in AS3.

## Compile check (no Unity needed)
`OUT=unity_stubs/out_<you> bash unity_stubs/build.sh` (run from `modkit/`) compiles all of `unity_template/Assets` against Unity stubs
(C# 9). Logs: `unity_stubs/out_<you>/runtime.log`. Fix every error in your files. Errors caused only by classes owned by others
not existing yet (CS0246/CS0103/CS1061 on their names) are expected while work is in progress – but if *you* reference a member of
another class, double-check its AS3 name and declared type in `decompiled/<Class>.as` so it will match their port.
Do not edit `Assets/Starlite`; if you need an API that is missing, use a small private helper in your own file and report it.
