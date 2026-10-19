// Starlite-only smoke test on FakeUnity: renders a test scene and checks hit testing / tweens.
using System;
using System.IO;
using Starlite;
using UnityEngine;

static class StarliteSmoke
{
    static int Main(string[] args)
    {
        FakeUnity.ResourcesDir = args[0];
        string outDir = args[1];
        Directory.CreateDirectory(outDir);
        var stage = Starling.Stage;
        var am = new AssetManager();
        am.enqueue("data/graphic/hd/title_BG.eag", "data/graphic/hd/title.ean", "data/graphic/hd/title.xml", "data/graphic/hd/game.ean", "data/graphic/hd/game.xml");
        bool loaded = false;
        am.loadQueue(r => { if (r == 1) loaded = true; });
        for (int i = 0; i < 3; i++) FakeUnity.Step();
        Console.WriteLine("loaded: " + loaded + " atlas=" + (am.getTextureAtlas("title") != null));

        var bg = new Starlite.Image(am.getTexture("title_BG"));
        stage.addChild(bg);
        var board = new Starlite.Image(am.getTexture("title_board"));
        board.x = 20; board.y = 60;
        stage.addChild(board);
        var logo = new Starlite.Image(am.getTexture("restaurant"));
        logo.pivotX = logo.width / 2; logo.pivotY = logo.height / 2;
        logo.x = 700; logo.y = 120; logo.rotation = 0.15f; logo.scaleX = logo.scaleY = 0.8f;
        stage.addChild(logo);
        // trimmed texture (asap frames have frameX/frameY)
        var steam = new Starlite.Image(am.getTexture("asap_0001"));
        steam.x = 600; steam.y = 300;
        stage.addChild(steam);
        var frameBox = new Quad(steam.width, steam.height, 0xff0000) { alpha = 0.25f };
        frameBox.x = 600; frameBox.y = 300;
        stage.addChild(frameBox);
        // container with pivot + clip rect
        var box = new Starlite.Sprite();
        box.x = 820; box.y = 420; box.pivotX = 50; box.pivotY = 50;
        box.addChild(new Quad(100, 100, 0x00ff00));
        var inner = new Quad(60, 60, 0x0000ff); inner.x = 70; inner.y = 70; box.addChild(inner);
        box.clipRect = new Starlite.Rectangle(0, 0, 100, 100);
        stage.addChild(box);
        // text
        var tf = new Starlite.TextField(400, 60, "Main Menu Fonts OK", "jenna_sue", 40, 0x663300) { x = 40, y = 520 };
        stage.addChild(tf);
        var bmf = new BitmapFont(AS3.embeddedTexture("MainMenuScreen_KomikandyGold"), AS3.embeddedXml("MainMenuScreen_KomikandyGoldXML"));
        Starlite.TextField.registerBitmapFont(bmf);
        var gold = new Starlite.TextField(500, 80, "GOAL 12345", "KomikandyGold", -1, 0xffffff) { x = 480, y = 500 };
        stage.addChild(gold);
        // hit testing / touch events
        int clicks = 0;
        logo.addEventListener(TouchEvent.TOUCH, (Starlite.Event e) => { if (((TouchEvent)e).getTouch(logo, TouchPhase.ENDED) != null) clicks++; });
        FakeUnity.Mouse = new Vector3(700, 600 - 120, 0); FakeUnity.Step();
        FakeUnity.MouseDown = FakeUnity.MouseHeld = true; FakeUnity.Step();
        FakeUnity.MouseHeld = false; FakeUnity.MouseUp = true; FakeUnity.Step();
        var hit = stage.hitTest(new Starlite.Point(700, 120), true);
        Console.WriteLine("hit=" + hit + " clicks=" + clicks + " boxBounds=" + box.getBounds(stage) + " logoBounds=" + logo.getBounds(stage));
        // tween
        TweenLite.to(board, 0.5f, new Vars { ["x"] = 120, ["ease"] = Expo.easeOut, ["onComplete"] = (Action)(() => Console.WriteLine("tween complete, x=" + board.x)) });
        for (int i = 0; i < 40; i++) FakeUnity.Step();
        FakeUnity.Screenshot(Path.Combine(outDir, "starlite_smoke.png"));
        Console.WriteLine("done; warnings=" + FakeUnity.Logs.FindAll(l => l.Contains("WARN")).Count);
        return clicks == 1 && hit == logo ? 0 : 1;
    }
}
