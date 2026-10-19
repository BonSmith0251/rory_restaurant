// Ported from TWindow.as (decompiled).
using Starlite;

public class TWindow
{
    public Global _global;
    public TextureAtlas windowsAtlas;
    public Quad _blackScreen;

    public TWindow()
    {
        this._global = Global.getInstance();
    }

    public void startLoad()
    {
        this._global.unPurgedAsset.enqueue("data/LevelEvent.xml");
        this._global.unPurgedAsset.enqueue("data/LevelSet.xml");
        this._global.unPurgedAsset.enqueue("data/Customer.xml");
        this._global.unPurgedAsset.enqueue("data/Tutorial_and_Dialogue.xml");
        this._global.unPurgedAsset.enqueue(this._global.graphicsPath + "window.ean", this._global.graphicsPath + "window.xml");
        this._global.unPurgedAsset.loadQueue((float ratio) =>
        {
            if (ratio == 1) this.init();
        });
    }

    public void init()
    {
        if (this._blackScreen != null) return;
        TGlobalClass.levelSetXML = this._global.unPurgedAsset.getXml("LevelSet");
        TGlobalClass.customerXML = this._global.unPurgedAsset.getXml("Customer");
        TGlobalClass.levelEventXML = this._global.unPurgedAsset.getXml("LevelEvent");
        TGlobalClass.dialogueXML = this._global.unPurgedAsset.getXml("Tutorial_and_Dialogue");
        this.windowsAtlas = this._global.unPurgedAsset.getTextureAtlas("window");
        this._blackScreen = new Quad(1024, 600, 0);
        this._blackScreen.alpha = 0.7f;
        this._blackScreen.visible = false;
        this._global._gameS.addChild(this._blackScreen);
    }
}
