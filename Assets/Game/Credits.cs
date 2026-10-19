// Ported from Credits.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class Credits
{
    public Sprite creditsContainer;
    public Global _global;
    public ExtendTextField versionText;
    public ExtendTextField creditsTitle;
    public Quad blackQuad;
    public cSprite origamingLogo;
    public ExtendTextField producerTitle;
    public ExtendTextField producerText;
    public ExtendTextField copyRight;
    public ExtendTextField gameConceptTitle;
    public ExtendTextField gameConceptText;
    public ExtendTextField leadArtistTitle;
    public ExtendTextField leadArtistText;
    public ExtendTextField programmersTitle;
    public ExtendTextField programmersText;
    public ExtendTextField graphicArtistTitle;
    public ExtendTextField graphicArtisText;
    public ExtendTextField QATitle;
    public ExtendTextField QAText;
    public ExtendTextField BGM_SFXTitle;
    public ExtendTextField BGM_SFXText;
    public ExtendTextField BGM_SFXInfo;

    public Credits()
    {
    }

    public void init()
    {
        this.creditsContainer = new Sprite();
        this._global._gameS.addChild(this.creditsContainer);
        this.creditsContainer.addEventListener(TouchEvent.TOUCH, this.touchCredits);
        this.blackQuad = new Quad(1024, 600, 0);
        this.blackQuad.alpha = 0.85f;
        this.creditsContainer.addChild(this.blackQuad);
        this.creditsTitle = new ExtendTextField(400, 150, "CREDITS", "Pfennig", 60, 16777215, true);
        this.creditsTitle.hAlign = "center";
        this.creditsTitle.pivotX = 200;
        this.creditsTitle.pivotY = 75;
        this.creditsTitle.x = 512;
        this.creditsTitle.y = 90;
        this.creditsContainer.addChild(this.creditsTitle);
        this.producerTitle = new ExtendTextField(250, 100, "PRODUCERS", "Pfennig", 30, 16777215, true);
        this.producerTitle.color = Color.RED;
        this.producerTitle.hAlign = "left";
        this.producerTitle.x = 125;
        this.producerTitle.y = 120;
        this.creditsContainer.addChild(this.producerTitle);
        this.producerText = new ExtendTextField(250, 100, "Andre Yu\nRobert YKG", "Pfennig", 25, 16777215, true);
        this.producerText.hAlign = "left";
        this.producerText.x = 125;
        this.producerText.y = this.producerTitle.y + 57;
        this.creditsContainer.addChild(this.producerText);
        this.origamingLogo = new cSprite(this._global._mainMenuScreen.titleAtlas.getTexture("origaminglogo"));
        this.origamingLogo.x = 400;
        this.origamingLogo.y = 140;
        this.creditsContainer.addChild(this.origamingLogo);
        this.copyRight = new ExtendTextField(600, 50, "All rights reserved. Origaming Media (c) 2015", "Pfennig", 20, 16777215, true);
        this.copyRight.hAlign = "left";
        this.copyRight.skewX = 0.4f;
        this.copyRight.italic = true;
        this.copyRight.x = this.origamingLogo.x;
        this.copyRight.y = this.origamingLogo.y + 50;
        this.creditsContainer.addChild(this.copyRight);
        this.gameConceptTitle = new ExtendTextField(250, 100, "GAME CONCEPT", "Pfennig", 30, 16777215, true);
        this.gameConceptTitle.color = Color.RED;
        this.gameConceptTitle.hAlign = "left";
        this.gameConceptTitle.x = 125;
        this.gameConceptTitle.y = 260;
        this.creditsContainer.addChild(this.gameConceptTitle);
        this.gameConceptText = new ExtendTextField(150, 50, "Andre Yu", "Pfennig", 25, 16777215, true);
        this.gameConceptText.hAlign = "left";
        this.gameConceptText.x = 125;
        this.gameConceptText.y = this.gameConceptTitle.y + 60;
        this.creditsContainer.addChild(this.gameConceptText);
        this.leadArtistTitle = new ExtendTextField(250, 100, "LEAD ARTIST", "Pfennig", 30, 16777215, true);
        this.leadArtistTitle.color = Color.RED;
        this.leadArtistTitle.hAlign = "left";
        this.leadArtistTitle.x = this.origamingLogo.x;
        this.leadArtistTitle.y = this.gameConceptTitle.y;
        this.creditsContainer.addChild(this.leadArtistTitle);
        this.leadArtistText = new ExtendTextField(150, 50, "Robert YKG", "Pfennig", 25, 16777215, true);
        this.leadArtistText.hAlign = "left";
        this.leadArtistText.x = this.origamingLogo.x;
        this.leadArtistText.y = this.gameConceptText.y;
        this.creditsContainer.addChild(this.leadArtistText);
        this.programmersTitle = new ExtendTextField(250, 100, "PROGRAMMERS", "Pfennig", 30, 16777215, true);
        this.programmersTitle.color = Color.RED;
        this.programmersTitle.hAlign = "left";
        this.programmersTitle.x = 125;
        this.programmersTitle.y = 360;
        this.creditsContainer.addChild(this.programmersTitle);
        this.programmersText = new ExtendTextField(300, 150, "Wilson\nSherson Watson\nAndri", "Pfennig", 25, 16777215, true);
        this.programmersText.hAlign = "left";
        this.programmersText.vAlign = "top";
        this.programmersText.x = 125;
        this.programmersText.y = this.programmersTitle.y + 65;
        this.creditsContainer.addChild(this.programmersText);
        this.graphicArtistTitle = new ExtendTextField(350, 100, "GRAPHIC ARTIST", "Pfennig", 30, 16777215, true);
        this.graphicArtistTitle.color = Color.RED;
        this.graphicArtistTitle.hAlign = "left";
        this.graphicArtistTitle.x = this.origamingLogo.x;
        this.graphicArtistTitle.y = this.programmersTitle.y;
        this.creditsContainer.addChild(this.graphicArtistTitle);
        this.graphicArtisText = new ExtendTextField(200, 200, "Alfa Lui\nHandrison", "Pfennig", 25, 16777215, true);
        this.graphicArtisText.hAlign = "left";
        this.graphicArtisText.x = this.origamingLogo.x;
        this.graphicArtisText.y = this.graphicArtistTitle.y;
        this.creditsContainer.addChild(this.graphicArtisText);
        // QA / BGM & SFX fields are created but (as in the original) never added to the container.
        this.QATitle = new ExtendTextField(100, 100, "QA", "Pfennig", 30, 16777215, true);
        this.QATitle.color = Color.RED;
        this.QATitle.hAlign = "left";
        this.QATitle.x = this.leadArtistTitle.x + 280;
        this.QATitle.y = this.graphicArtistTitle.y;
        this.QAText = new ExtendTextField(150, 50, "Lilis", "Pfennig", 25, 16777215, true);
        this.QAText.hAlign = "left";
        this.QAText.x = this.QATitle.x;
        this.QAText.y = this.QATitle.y + 63;
        this.BGM_SFXTitle = new ExtendTextField(250, 100, "BGM and SFX", "Pfennig", 30, 16777215, true);
        this.BGM_SFXTitle.color = Color.RED;
        this.BGM_SFXTitle.hAlign = "left";
        this.BGM_SFXTitle.x = this.QATitle.x;
        this.BGM_SFXTitle.y = this.leadArtistTitle.y;
        this.BGM_SFXText = new ExtendTextField(350, 50, "Iguatemi Canfield Garin", "Pfennig", 25, 16777215, true);
        this.BGM_SFXText.hAlign = "left";
        this.BGM_SFXText.x = this.BGM_SFXTitle.x;
        this.BGM_SFXText.y = this.leadArtistText.y;
        this.BGM_SFXInfo = new ExtendTextField(350, 50, "Professional Audioworks", "Pfennig", 20, 16777215, true);
        this.BGM_SFXInfo.hAlign = "left";
        this.BGM_SFXInfo.x = this.BGM_SFXTitle.x + 8;
        this.BGM_SFXInfo.y = this.BGM_SFXText.y + 30;
        this.BGM_SFXInfo.skewX = 0.4f;
        this.versionText = new ExtendTextField(600, 60, Settings.Name + "  v. " + Settings.Version, "Pfennig", 20, 16777215, true);
        this.versionText.hAlign = "left";
        this.versionText.x = 122;
        this.versionText.y = 540;
        this.versionText.touchable = false;
        this.creditsContainer.addChild(this.versionText);
        this.creditsContainer.visible = false;
    }

    public void Kill()
    {
        this.creditsContainer.removeEventListener(TouchEvent.TOUCH, this.touchCredits);
        this.creditsTitle.removeFromParent(true);
        this.blackQuad.removeFromParent(true);
        this.origamingLogo.removeFromParent(true);
        this.producerTitle.removeFromParent(true);
        this.producerText.removeFromParent(true);
        this.copyRight.removeFromParent(true);
        this.gameConceptTitle.removeFromParent(true);
        this.gameConceptText.removeFromParent(true);
        this.leadArtistTitle.removeFromParent(true);
        this.leadArtistText.removeFromParent(true);
        this.programmersTitle.removeFromParent(true);
        this.programmersText.removeFromParent(true);
        this.graphicArtistTitle.removeFromParent(true);
        this.graphicArtisText.removeFromParent(true);
        this.QATitle.removeFromParent(true);
        this.QAText.removeFromParent(true);
        this.creditsTitle = null;
        this.blackQuad = null;
        this.origamingLogo = null;
        this.producerTitle = null;
        this.producerText = null;
        this.copyRight = null;
        this.gameConceptTitle = null;
        this.gameConceptText = null;
        this.leadArtistTitle = null;
        this.leadArtistText = null;
        this.programmersTitle = null;
        this.programmersText = null;
        this.graphicArtistTitle = null;
        this.graphicArtisText = null;
        this.QATitle = null;
        this.QAText = null;
        this.creditsContainer.removeFromParent(true);
        this.creditsContainer = null;
    }

    public void touchCredits(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this.creditsContainer.visible = false;
        }
    }
}
