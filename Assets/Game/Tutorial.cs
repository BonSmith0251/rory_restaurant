// Ported from Tutorial.as (decompiled). Tutorial / dialogue system driven by Tutorial_and_Dialogue.xml.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class Tutorial
{
    public const int TUTORIAL_COUNT = 30;
    public const int OPENING_STORY = 0;
    public const int LEVEL_1 = 1;
    public const int AFTER_LEVEL_1 = 2;
    public const int AFTER_LEVEL_2 = 3;
    public const int LEVEL_4 = 4;
    public const int LEVEL_5 = 5;
    public const int LEVEL_7 = 6;
    public const int AFTER_LEVEL_7 = 7;
    public const int LEVEL_8 = 8;
    public const int AFTER_LEVEL_8 = 9;
    public const int LEVEL_11 = 10;
    public const int LEVEL_12 = 11;
    public const int LEVEL_13 = 12;
    public const int LEVEL_14 = 13;
    public const int LEVEL_16 = 14;
    public const int LEVEL_17 = 15;
    public const int LEVEL_18 = 16;
    public const int LEVEL_19 = 17;
    public const int LEVEL_21 = 18;
    public const int LEVEL_22 = 19;
    public const int LEVEL_25 = 20;
    public const int LEVEL_26 = 21;
    public const int LEVEL_27 = 22;
    public const int LEVEL_28 = 23;
    public const int LEVEL_31 = 24;
    public const int LEVEL_32 = 25;
    public const int AFTER_LEVEL_34 = 26;

    public Global _global;
    public int currentDialogueID;
    public int dialogueCounter;
    public TextureAtlas atlas;
    public cSprite textBox;
    public cSprite ava;
    public cSprite arrow;
    public TNonStateButton button;
    public ExtendTextField text;
    public bool blockCustomerIn;
    // AS3 Arrays of {col, row} object literals
    public List<Dictionary<string, object>> highlightedAr;
    public List<Dictionary<string, object>> highlightedAr2;

    public Tutorial()
    {
        this.highlightedAr = new List<Dictionary<string, object>>();
        this.highlightedAr2 = new List<Dictionary<string, object>>();
    }

    public void init()
    {
        this._global = Global.getInstance();
        this.atlas = TGlobalClass._windowDisplay.windowsAtlas;
        this.textBox = new cSprite(this.atlas.getTexture("textbox"));
        this.textBox.pivotX = this.textBox.width / 2;
        this.textBox.pivotY = this.textBox.height / 2;
        this.textBox.scaleX = 1.2f;
        this.textBox.visible = false;
        this.textBox.touchable = false;
        this._global._gameS.addChild(this.textBox);
        this.ava = new cSprite(this.atlas.getTexture("rory"));
        this.ava.pivotY = this.ava.height;
        this.ava.visible = false;
        this.ava.touchable = false;
        this._global._gameS.addChild(this.ava);
        this.arrow = new cSprite(this.atlas.getTexture("arrow"));
        this.arrow.pivotX = this.arrow.width / 2;
        this.arrow.pivotY = this.arrow.height / 2;
        this.arrow.visible = false;
        this.arrow.touchable = false;
        this._global._gameS.addChild(this.arrow);
        this.button = new TNonStateButton(this.atlas.getTexture("small_btn_blue"), "Next", "bromine", 20, 0);
        this.button.pivotX = this.button.width / 2;
        this.button.pivotY = this.button.height / 2;
        this.button.visible = false;
        this._global._gameS.addChild(this.button);
        this.button.addEventListener(TouchEvent.TOUCH, this.touchButton);
        this.text = new ExtendTextField((int)(this.textBox.width - 135), (int)(this.textBox.height - 10), "", "pfennig", 20, 0);
        this.text.pivotX = this.textBox.width / 2 - 115;
        this.text.pivotY = this.textBox.height / 2 - 5;
        this.text.visible = false;
        this.text.touchable = false;
        this.text.hAlign = "center";
        this.text.vAlign = "center";
        this._global._gameS.addChild(this.text);
        this._global._gameS.addEventListener(TutorialEvent.TRIGGER_NEXT, this.onTriggerNext);
    }

    public void bringTutorialToTop()
    {
        this._global._gameS.setChildIndex(this.textBox, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.ava, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.arrow, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.button, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.text, this._global._gameS.numChildren - 1);
    }

    // ---- small helpers for AS3 dynamic access (Object literals / E4X)
    static object info(TutorialEvent e, string key) =>
        e.infos != null && e.infos.TryGetValue(key, out var v) ? v : null;

    static int hcol(List<Dictionary<string, object>> ar, int i) => AS3.toInt(ar[i]["col"]);
    static int hrow(List<Dictionary<string, object>> ar, int i) => AS3.toInt(ar[i]["row"]);

    /// <summary>TGlobalClass.dialogueXML.dialogue[id].text[counter] (null when missing, like E4X undefined).</summary>
    static XML dialogueText(int id, int counter)
    {
        XML d = TGlobalClass.dialogueXML["dialogue"][id];
        return d != null ? d["text"][counter] : null;
    }

    /// <summary>E4X attribute assignment (node.@name = value); Starlite XML is read-only, so write the backing element.</summary>
    static void setXmlAttr(XML node, string name, string value)
    {
        node?.setAttribute(name, value);
    }

    public void onTriggerNext(Event arg1)
    {
        var e = (TutorialEvent)arg1;
        int _local2 = 0;
        int _local3 = 0;
        if (this.currentDialogueID == -1) return;

        if (e.actionName == "highlight")
        {
            _local2 = AS3.toInt(info(e, "col"));
            _local3 = AS3.toInt(info(e, "row"));
            if (this.highlightedAr2.Count >= 3) this.highlightedAr2 = new List<Dictionary<string, object>>();
            this.highlightedAr2.Add(new Dictionary<string, object> { ["col"] = _local2, ["row"] = _local3 });
            if (this.highlightedAr2.Count >= 3)
            {
                var h = this.highlightedAr2;
                if (hrow(h, 0) == hrow(h, 1) && hrow(h, 1) == hrow(h, 2))
                {
                    if (Mathf.Abs(hcol(h, 0) - hcol(h, 1)) == 1) { _local2 = hcol(h, 2); _local3 = hrow(h, 2); }
                    else if (Mathf.Abs(hcol(h, 0) - hcol(h, 2)) == 1) { _local2 = hcol(h, 1); _local3 = hrow(h, 1); }
                    else { _local2 = hcol(h, 0); _local3 = hrow(h, 0); }
                }
                else if (hcol(h, 0) == hcol(h, 1) && hcol(h, 1) == hcol(h, 2))
                {
                    if (Mathf.Abs(hrow(h, 0) - hrow(h, 1)) == 1) { _local2 = hcol(h, 2); _local3 = hrow(h, 2); }
                    else if (Mathf.Abs(hrow(h, 0) - hrow(h, 2)) == 1) { _local2 = hcol(h, 1); _local3 = hrow(h, 1); }
                    else { _local2 = hcol(h, 0); _local3 = hrow(h, 0); }
                }
                else if (!(hrow(h, 0) == hrow(h, 1) || hrow(h, 0) == hrow(h, 2) || hcol(h, 0) == hcol(h, 1) || hcol(h, 0) == hcol(h, 2)))
                {
                    _local2 = hcol(h, 0); _local3 = hrow(h, 0);
                }
                else if (!(hrow(h, 1) == hrow(h, 0) || hrow(h, 1) == hrow(h, 2) || hcol(h, 1) == hcol(h, 0) || hcol(h, 1) == hcol(h, 2)))
                {
                    _local2 = hcol(h, 1); _local3 = hrow(h, 1);
                }
                this._global.gameScreen.highlightX = (int)(this._global.gameScreen.tileAr[_local2][0].x + 40);
                this._global.gameScreen.highlightY = (int)this._global.gameScreen.tileAr[0][_local3].y;
            }
        }

        string a = e.actionName;
        int dc = this.dialogueCounter;
        if (this.currentDialogueID == LEVEL_1)
        {
            if (a == "clicked_ok" && dc == 1)
            {
                this._global.gameScreen.startLevel(1);
            }
            else if (a == "customer_in" && dc == 1)
            {
                this.nextTutorial();
            }
            else if (a == "clicked_ok" && dc == 2)
            {
                this._global.gameScreen.dispatchHighlightPosition = 3;
                this._global.gameScreen.highlightPossible();
                this._global.gameScreen.highlightPossibleTile = true;
            }
            else if (a == "matched" && dc == 3)
            {
                this.nextTutorial();
            }
            else if (a == "dish_completed" && dc == 4)
            {
                if (this.textBox.visible) this.hideTutorial();
                this.nextTutorial();
            }
            else if (a == "clicked_coin" && dc == 5)
            {
                this.blockCustomerIn = true;
                this.nextTutorial();
            }
            else if (a == "clicked_ok" && dc == 6)
            {
                this.blockCustomerIn = false;
                this._global.gameScreen.hasWindow = false;
                for (int _local4 = 0; _local4 < 3; _local4++)
                {
                    this._global.gameScreen.customer[_local4].tweenOut(this._global.gameScreen.customer[_local4].CUST_SATISFIED);
                }
            }
            else if (a == "customer_in" && AS3.toNumber(info(e, "position")) == 2 && dc == 6)
            {
                this.nextTutorial();
            }
            else if (a == "changed_priority" && dc == 7)
            {
                this.nextTutorial();
            }
        }
        else if (this.currentDialogueID == AFTER_LEVEL_2)
        {
            if (a == "clicked_recipe_shop" && dc == 1) this.nextTutorial();
            else if (a == "right_button" && dc == 3) this.nextTutorial();
            else if (a == "buy_item" && dc == 4) this.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_4)
        {
            if (a == "get_correct_ho_item" && dc == 4) this.nextTutorial();
            else if (a == "used_hint" && dc == 5) this.hideTutorial();
            else if (a == "get_correct_ho_item" && dc == 5) this.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_5)
        {
            if (a == "clicked_ok" && dc == 2) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_7)
        {
            if (a == "clicked_ok" && dc == 2)
            {
                this._global.gameScreen.startLevel();
            }
            else if (a == "special_customer_in" && dc == 2)
            {
                setXmlAttr(TGlobalClass.dialogueXML["dialogue"][LEVEL_7]["text"][3], "arrowY",
                    AS3.str(120 + AS3.toNumber(info(e, "position")) * 118));
                this.nextTutorial();
            }
        }
        else if (this.currentDialogueID == LEVEL_8)
        {
            if (a == "clicked_ok" && dc == 1) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == AFTER_LEVEL_8)
        {
            if (a == "clicked_ingredients" && dc == 2) this.nextTutorial();
            else if (a == "buy_item" && dc == 8) this.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_11)
        {
            if (a == "clicked_ok" && dc == 4) this._global._minigameDessert.startLevel();
            else if (a == "check_times_up" && dc == 4) this._global.tutorial.nextTutorial();
            else if (a == "touch_ingredient" && dc == 7 && AS3.toNumber(info(e, "id")) == 0) this._global.tutorial.nextTutorial();
            else if (a == "touch_ingredient" && dc == 8 && AS3.toNumber(info(e, "id")) == 1) this._global.tutorial.nextTutorial();
            else if (a == "touch_ingredient" && dc == 9 && AS3.toNumber(info(e, "id")) == 3) this._global.tutorial.nextTutorial();
            else if (a == "touch_ingredient" && dc == 10 && AS3.toNumber(info(e, "id")) == 4) this._global.tutorial.nextTutorial();
            else if (a == "touch_ingredient" && dc == 11 && AS3.toNumber(info(e, "id")) == 6) this._global.tutorial.hideTutorial();
            else if (a == "check_times_up" && dc == 11) this._global.tutorial.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_12)
        {
            if (a == "customer_in" && dc == 0)
            {
                this._global.gameScreen.hasWindow = false;
                this.nextTutorial();
            }
            else if (a == "clicked_ok" && dc == 2)
            {
                this._global.gameScreen.dispatchHighlightPosition = 3;
                this._global.gameScreen.highlightPossible();
                this._global.gameScreen.highlightPossibleTile = true;
            }
            else if (a == "matched" && dc == 3) this.nextTutorial();
            else if (a == "clicked_beverage" && dc == 4) this.nextTutorial();
            else if (a == "offered_beverage" && dc == 5) this.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_13)
        {
            if (a == "clicked_ok" && dc == 2) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_14)
        {
            if (a == "clicked_ok" && dc == 0) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_16)
        {
            if (a == "clicked_ok" && dc == 2) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_17)
        {
            if (a == "customer_in" && dc == 0)
            {
                this._global.gameScreen.hasWindow = false;
                this.nextTutorial();
            }
            else if (a == "clicked_ok" && dc == 2)
            {
                this._global.gameScreen.dispatchHighlightPosition = 3;
                this._global.gameScreen.highlightPossible();
                this._global.gameScreen.highlightPossibleTile = true;
            }
            else if (a == "matched" && dc == 3) this.nextTutorial();
            else if (a == "clicked_fries" && dc == 4) this.nextTutorial();
            else if (a == "offered_fries" && dc == 5) this.nextTutorial();
        }
        else if (this.currentDialogueID == LEVEL_19)
        {
            if (a == "clicked_ok" && dc == 0) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_21)
        {
            if (a == "clicked_ok" && dc == 0) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_22)
        {
            if (a == "clicked_ok" && dc == 1) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_25)
        {
            if (a == "clicked_ok" && dc == 0) this._global._minigameDessert.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_26)
        {
            if (a == "customer_in" && dc == 0)
            {
                this._global.gameScreen.hasWindow = false;
                this.nextTutorial();
            }
            else if (a == "highlight" && dc == 0)
            {
                _local2 = AS3.toInt(info(e, "col"));
                _local3 = AS3.toInt(info(e, "row"));
                if (this.highlightedAr.Count >= 3) this.highlightedAr = new List<Dictionary<string, object>>();
                this.highlightedAr.Add(new Dictionary<string, object> { ["col"] = _local2, ["row"] = _local3 });
                if (this.highlightedAr.Count < 3) return;
                var h = this.highlightedAr;
                if (hrow(h, 0) == hrow(h, 1) && hrow(h, 1) == hrow(h, 2))
                {
                    if (Mathf.Abs(hcol(h, 0) - hcol(h, 1)) == 1) { _local2 = hcol(h, 0); _local3 = hrow(h, 0); }
                    else { _local2 = hcol(h, 2); _local3 = hrow(h, 2); }
                }
                else if (hcol(h, 0) == hcol(h, 1) && hcol(h, 1) == hcol(h, 2))
                {
                    if (Mathf.Abs(hrow(h, 0) - hrow(h, 1)) == 1) { _local2 = hcol(h, 0); _local3 = hrow(h, 0); }
                    else { _local2 = hcol(h, 2); _local3 = hrow(h, 2); }
                }
                else if (!(hrow(h, 2) == hrow(h, 0) || hrow(h, 2) == hrow(h, 1) || hcol(h, 2) == hcol(h, 0) || hcol(h, 2) == hcol(h, 1)))
                {
                    _local2 = hcol(h, 0); _local3 = hrow(h, 0);
                }
                var gs = this._global.gameScreen;
                gs.tileAr[_local2][_local3].frozen = true;
                for (int _local5 = 0; _local5 < gs.frozenTileAr.Count; _local5++)
                {
                    if (gs.frozenTileAr[_local5].visible) continue;
                    gs.frozenTileAr[_local5].visible = true;
                    gs.frozenTileAr[_local5].x = gs.tileAr[_local2][_local3].x;
                    gs.frozenTileAr[_local5].y = gs.tileAr[_local2][_local3].y;
                    gs.frozenTileAr[_local5].attachedCol = _local2;
                    gs.frozenTileAr[_local5].attachedRow = _local3;
                    gs.tileAr[_local2][_local3].frozenTileIdx = gs.frozenTileAr[_local5].idx;
                    break;
                }
                gs.currentFrozenTile = gs.currentFrozenTile + 1;
            }
            else if (a == "clicked_ok" && dc == 2)
            {
                this._global.gameScreen.dispatchHighlightPosition = 3;
                this._global.gameScreen.highlightPossible();
                this._global.gameScreen.highlightPossibleTile = true;
            }
            else if (a == "matched" && dc == 3)
            {
                this.nextTutorial();
            }
        }
        else if (this.currentDialogueID == LEVEL_27)
        {
            if (a == "customer_in" && dc == 0)
            {
                this.nextTutorial();
            }
            else if (a == "start_torch" && dc == 2)
            {
                this.nextTutorial();
                this._global.gameScreen.hasWindow = false;
            }
        }
        else if (this.currentDialogueID == LEVEL_28)
        {
            if (a == "clicked_ok" && dc == 0) this._global.gameScreen.startLevel();
        }
        else if (this.currentDialogueID == LEVEL_31)
        {
            if (a == "customer_in" && dc == 0)
            {
                this.nextTutorial();
                this._global.gameScreen.hasWindow = false;
            }
            else if (a == "give_cookies" && dc == 2)
            {
                this.nextTutorial();
            }
        }
        else if (this.currentDialogueID == AFTER_LEVEL_34)
        {
            if (a == "clicked_hardware" && dc == 1) this.nextTutorial();
        }
    }

    public bool canClickThisButton(int arg1)
    {
        if (this.currentDialogueID == AFTER_LEVEL_2)
        {
            if (this.dialogueCounter == 1 && !(arg1 == Kitchen.RECIPE_ID)) return false;
            if (this.dialogueCounter == 3 && !(arg1 == Kitchen.RIGHT_BUTTON)) return false;
            if (this.dialogueCounter == 4 && !(arg1 == Kitchen.BUY_BUTTON)) return false;
        }
        else if (this.currentDialogueID == AFTER_LEVEL_8)
        {
            if (this.dialogueCounter == 2 && !(arg1 == Kitchen.SHOP_ID)) return false;
            if (this.dialogueCounter == 8 && !(arg1 == Kitchen.BUY_BUTTON)) return false;
        }
        else if (this.currentDialogueID == LEVEL_11)
        {
            if (this.dialogueCounter == 7 && !(arg1 == 0)) return false;
            if (this.dialogueCounter == 8 && !(arg1 == 1)) return false;
            if (this.dialogueCounter == 9 && !(arg1 == 3)) return false;
            if (this.dialogueCounter == 10 && !(arg1 == 4)) return false;
            if (this.dialogueCounter == 11 && !(arg1 == 6)) return false;
        }
        else if (this.currentDialogueID == AFTER_LEVEL_34)
        {
            if (this.dialogueCounter == 1 && !(arg1 == Kitchen.HARDWARE_ID)) return false;
        }
        return true;
    }

    public bool checkTut(int arg1, int arg2, bool arg3 = false)
    {
        if (TSavegame.currentPlayer.passedDays == arg2
            && (this._global.currentSceneID == this._global.KITCHEN_ID || TGlobalClass.currentPlayedLevel == arg2)
            && (!TSavegame.currentPlayer.tutorialShowed[arg1] || arg3))
        {
            this.showTutorial(arg1);
            TSavegame.currentPlayer.tutorialShowed[arg1] = true;
            return true;
        }
        return false;
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ok"));
            XML node = dialogueText(this.currentDialogueID, this.dialogueCounter);
            if (node == null || (string)node["@blockNext"] != "true")
            {
                this.nextTutorial();
            }
            else
            {
                this.hideTutorial();
            }
        }
    }

    public void showTutorial(int arg1, int arg2 = 0)
    {
        this.currentDialogueID = arg1;
        this.dialogueCounter = arg2;
        this.hideTutorial();
        XML _local3 = dialogueText(arg1, arg2);
        if (_local3.toString() == "(start level)")
        {
            this._global.gameScreen.startLevel();
            return;
        }
        this.textBox.visible = true;
        this.ava.visible = true;
        if ((string)_local3.attribute("arrowY") != "") this.arrow.visible = true;
        if ((string)_local3.attribute("proceedType") == "button") this.button.visible = true;
        this.text.visible = true;
        this.textBox.x = _local3.attribute("posX").toFloat();
        this.textBox.y = _local3.attribute("posY").toFloat();
        this.arrow.x = this.processArrowPosition(_local3.attribute("arrowX"));
        this.arrow.y = this.processArrowPosition(_local3.attribute("arrowY"));
        this.button.x = this.textBox.x;
        this.button.y = this.textBox.y + 110;
        this.ava.x = this.textBox.x - this.textBox.pivotX * this.textBox.scaleX + 5;
        this.ava.y = this.textBox.y + this.textBox.pivotY - 5;
        this.text.x = this.textBox.x;
        this.text.y = this.textBox.y;
        this.arrow.scaleX = _local3.attribute("arrowDirection").toInt();
        this.tweenArrow(!(_local3.attribute("arrowDirection").toFloat() == 1));
        this.text.text = _local3.toString();
        this.text.text = this.text.text.replace("(player)", TSavegame.currentPlayer.name);
        if (this._global.currentSceneID == this._global.GAMESCREEN_ID)
        {
            this.text.text = this.text.text.replace("(juice percentage)", this._global.gameScreen.juicerPercentage.text);
        }
        this.button._text.text = _local3.attribute("buttonText").toString();
    }

    public int processArrowPosition(string arg1)
    {
        int _local2 = 0;
        int _local3 = 0;
        if (arg1 == "HOITEMX")
        {
            var ho = this._global._minigameHO._HOClass;
            _local2 = (int)(ho.itemPosXAr[ho.listedItemAr[0]] - 40);
            _local3 = (int)ho.itemPosYAr[ho.listedItemAr[0]];
            if (_local3 > 300) this.textBox.y = 250;
            else this.textBox.y = 500;
            if (_local2 > 512)
            {
                this.textBox.x = 335;
            }
            else
            {
                this.textBox.x = 670;
                setXmlAttr(TGlobalClass.dialogueXML["dialogue"][Tutorial.LEVEL_4]["text"][4], "arrowDirection", "-1");
                _local2 = _local2 + 100;
            }
            return _local2;
        }
        if (arg1 == "HOITEMY")
        {
            var ho = this._global._minigameHO._HOClass;
            return (int)(ho.itemPosYAr[ho.listedItemAr[0]] + ho.itemDisplayAr[ho.listedItemAr[0]].height / 2);
        }
        if (arg1 == "TILEX") return this._global.gameScreen.highlightX;
        if (arg1 == "TILEY")
        {
            if (this._global.gameScreen.highlightY > 300) this.textBox.y = 250;
            else this.textBox.y = 500;
            return this._global.gameScreen.highlightY;
        }
        if (arg1 == "TORCHX") return (int)(this._global.gameScreen.tileAr[1][1].x + 40);
        if (arg1 == "TORCHY") return (int)this._global.gameScreen.tileAr[1][1].y;
        if (arg1 == "COOKIEX") return (int)(this._global.gameScreen.tileAr[1][1].x + 40);
        if (arg1 == "COOKIEY") return (int)this._global.gameScreen.tileAr[1][1].y;
        return AS3.toInt(arg1);
    }

    public void tweenArrow(bool arg1)
    {
        bool reverse = arg1;
        TweenLite.to(this.arrow, 0.6f, new Vars
        {
            ["scaleX"] = 0.8f * (reverse ? -1 : 1),
            ["scaleY"] = 0.95f,
            ["x"] = this.arrow.x - 40 * (reverse ? -1 : 1),
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.arrow, 0.5f, new Vars
                {
                    ["scaleX"] = 1 * (reverse ? -1 : 1),
                    ["scaleY"] = 1,
                    ["x"] = this.arrow.x + 40 * (reverse ? -1 : 1),
                    ["onComplete"] = (Action)(() => { this.tweenArrow(reverse); }),
                });
            }),
        });
    }

    public void nextTutorial()
    {
        this.dialogueCounter = this.dialogueCounter + 1;
        if (dialogueText(this.currentDialogueID, this.dialogueCounter) != null)
        {
            this.showTutorial(this.currentDialogueID, this.dialogueCounter);
        }
        else
        {
            this.currentDialogueID = -1;
            this.hideTutorial();
        }
    }

    public void hideTutorial()
    {
        this.textBox.visible = false;
        this.ava.visible = false;
        this.arrow.visible = false;
        this.button.visible = false;
        this.text.visible = false;
        TweenLite.killTweensOf(this.arrow);
    }

    public void kill()
    {
        // (sic) the AS3 removes the listener from textBox, not from _gameS where it was added
        this.textBox.removeEventListener(TutorialEvent.TRIGGER_NEXT, this.onTriggerNext);
        this.textBox.removeFromParent(true);
        this.textBox = null;
        this.ava.removeFromParent(true);
        this.ava = null;
        TweenLite.killTweensOf(this.arrow);
        this.arrow.removeFromParent(true);
        this.arrow = null;
        this.text.removeFromParent(true);
        this.text = null;
    }
}
