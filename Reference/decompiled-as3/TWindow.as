package {
class TWindow {
    var _global:Global;
    var windowsAtlas:TextureAtlas;
    var _blackScreen:Quad;

    function TWindow():* {
        this._global = Global.getInstance();
        super();
    }

    function startLoad():void {
        this._global.unPurgedAsset.enqueue("data/LevelEvent.xml");
        this._global.unPurgedAsset.enqueue("data/LevelSet.xml");
        this._global.unPurgedAsset.enqueue("data/Customer.xml");
        this._global.unPurgedAsset.enqueue("data/Tutorial_and_Dialogue.xml");
        this._global.unPurgedAsset.enqueue(this._global.graphicsPath + "window.ean", this._global.graphicsPath + "window.xml");
        this._global.unPurgedAsset.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L14;
            init();
          L14:
        });
    }

    function init():void {
        if (this._blackScreen) goto L159;
        TGlobalClass.levelSetXML = this._global.unPurgedAsset.getXml("LevelSet");
        TGlobalClass.customerXML = this._global.unPurgedAsset.getXml("Customer");
        TGlobalClass.levelEventXML = this._global.unPurgedAsset.getXml("LevelEvent");
        TGlobalClass.dialogueXML = this._global.unPurgedAsset.getXml("Tutorial_and_Dialogue");
        this.windowsAtlas = this._global.unPurgedAsset.getTextureAtlas("window");
        this._blackScreen = new Quad(1024, 600, 0);
        this._blackScreen.alpha = 0.7;
        this._blackScreen.visible = false;
        this._global._gameS.addChild(this._blackScreen);
      L159:
    }
}
}
