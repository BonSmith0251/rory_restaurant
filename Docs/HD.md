# 1920x1080 / HD

The game runs at **1920x1080 (16:9)**: full screen by default, or a 1920x1080 window (Options: Windowed). On
other monitors the 1920x1080 image is scaled to the screen.

## How the screen is laid out

* All layout still uses the original game's units: the **1024x600 design area** (the `Stage` of every scene).
  Positions in builders, tutorials and data stay exactly as in the original game.
* The canvas shows a **16:9 viewport of 1066.67 x 600 units** around the stage (`UIStage.ViewWidth`). At 1920x1080
  one unit is exactly 1.8 pixels.
* The extra 21.33 units left and right (`UIStage.SideMargin`) are filled automatically:
  * full-screen backgrounds (Art/Backgrounds, Art/Branding) placed at (0, 0): a 16:9 image fills the whole view;
    a 1024x600 original gets mirrored edge strips (`ViewEdgeFill`);
  * full-stage quads (dimming, touch areas) and the popup backdrop cover the whole view.
  The HUD keeps its original positions (pinning it to the screen edges would move it away from the tutorial
  arrows that point at it).

## Supplying HD art

Replace an image with a higher-resolution version **under the same file name** (same aspect ratio). Nothing else
is needed: the importer gives it a higher pixels-per-unit so it keeps its layout size and is drawn sharper.

* **Sprites** (`Art/Sprites/<atlas>/<name>.png`): the layout size is the original pixel size, recorded in
  `Art/Sprites/LogicalSizes.json`. A 2x image of a 100x50 sprite (200x100) still occupies 100x50 units.
  For a *new* sprite, add it at the size it should occupy, then run `python Tools/native/record_sprite_sizes.py`
  (it records new files only; recorded sizes never change).
* **Backgrounds** (`Art/Backgrounds`, `Art/Branding`): always span the 600-unit height. A 16:9 image
  (e.g. 1920x1080 or 3840x2160) fills the whole 16:9 view; a 1024x600-shaped image gets the mirrored side strips.
  `title_BG.png` (1672x941) is already a 16:9 HD background.
* New art with a different shape or padding (e.g. a square 1254x1254 picture of a dish):
  `python Tools/native/fit_hd_art.py <new.png> kitchen/cheese.png kitchen/cheese_small.png game/keju.png`
  fits it into the original's content area and aspect ratio at 2x (`--portrait` for customer busts: fills the
  height, bottom aligned). Most ingredients and dishes (`kitchen/*`, their `_small` versions and `game/*`) were
  replaced this way.
* Customers: `python Tools/native/customer_portraits.py <sheet>` cuts the 13 busts out of a portrait sheet and
  draws them at one scale into one frame per game (`game/char_N` 88x100, `make_dessert/char_N` 138x166, written to
  `LogicalSizes.json`), so every customer appears at the same place.
* Recommended sizes: 2x for sprites (sharp at 1080p: 1 unit = 1.8 px), 1920x1080 (or larger) for backgrounds.
* Texture size limit is 4096; sprite atlases pack HD sprites like the others.
* After replacing art: `Rory > Build > All Prefabs and Scenes` (layouts that depend on image sizes are regenerated).
* Not covered yet: the gold "Komikandy" bitmap fonts (they need new font files at 2x); TextMeshPro text is already
  sharp at any resolution.
