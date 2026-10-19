// Ported from HOClass.as (decompiled). Hidden-object logic: item data, listing, picking, found-item tweens, hints.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class HOClass
{
    public int targetIDBegan;
    public Global _global;
    public List<string> itemNameAr;
    public List<int> itemTypeAr;
    public List<float> itemPosXAr;
    public List<float> itemPosYAr;
    public List<string> itemFileNameAr;
    public List<HOObjects> itemHOAr;
    public List<int> itemAsNormal;
    public List<int> itemCoins;
    public List<Sprite> itemDisplayAr;
    public List<List<int>> itemMultiples;
    public int itemLogoIdx;
    public List<ExtendTextField> itemTextUIAr;
    public List<HOObjects> itemUIAr;
    public Quad listedItemsPlace;
    public HOObjects HOItem;
    public ExtendTextField HOText;
    public PixelHitArea pixelHitArea;
    public XML itemsData;
    public HOObjects revealedItem;
    public bool missClick;
    public int missClickCount;
    public int allowedMissClick;
    public int itemCounting;
    public int maxItems;
    public int totalItems;
    public bool itemTweening;
    public int loadedPack;
    public bool getRevealedItem;
    public int itemFoundCount;
    public int comboCount;
    public int soundCombo;
    public int revealedDisplayIdx;
    public bool onClick;
    public int itemIdx;
    public bool onScaleBack;
    public List<int> listedItemAr;
    public List<int> itemFoundAr;
    public List<bool> occupiedPlace;
    public List<int> occupiedIdx;
    public const int MAX_DISPLAYED_ITEMS = 3;
    public const int AS_HO_OBJECT_ID = 1;
    public const int AS_BG_ID = 2;
    public const int AS_BG2_ID = 7;
    public const int AS_COIN_ID = 4;
    public const int AS_LOGO_ID = 5;
    public const int AS_MULTIPLE_ID = 6;
    public int multipleItemsTweening;
    public const float HINT_X = 940;
    public const float HINT_Y = 60;
    public int revealedIdx;
    public float centerX = 150;
    public float centerY = 150;
    public float radiusX = 80;
    public float radiusY = 80;
    public float angleStep = 0.02f;
    public float twoPI = 6.283185307179586f;
    public float currentAngle = 0.25f;

    public HOClass()
    {
        this._global = Global.getInstance();
        this.occupiedPlace = new List<bool> { false, false, false };
        this.occupiedIdx = AS3Extensions.filled<int>(3, i => 0);
    }

    public void Kill()
    {
        int count = this.itemDisplayAr.Count;
        for (int i = 0; i < count; i++)
        {
            TweenLite.killTweensOf(this.itemDisplayAr[i]);
            this.itemDisplayAr[i].removeEventListener(TouchEvent.TOUCH, this.touchHOObjects);
            this.itemDisplayAr[i].removeFromParent(true);
            this.itemDisplayAr[i] = null;
            this.itemHOAr[i].Kill();
            this.itemHOAr[i].display.removeFromParent(true);
            this.itemHOAr[i] = null;
        }
        this.itemDisplayAr.Clear();
        this.itemHOAr.Clear();
        count = this.itemTextUIAr.Count;
        for (int i = 0; i < count; i++)
        {
            this.itemTextUIAr[i].removeFromParent(true);
            this.itemTextUIAr[i] = null;
        }
        this.itemTextUIAr.Clear();
        this.itemFileNameAr.Clear();
        this.itemNameAr.Clear();
        this.itemPosXAr.Clear();
        this.itemPosYAr.Clear();
        this.itemTypeAr.Clear();
        this.pixelHitArea.dispose();
        this.pixelHitArea = null;
        if (this.HOText != null)
        {
            this.HOText.removeFromParent(true);
            this.HOText = null;
        }
        this.HOItem.display.filter = null;
        this.HOItem.display.removeFromParent(true);
        this.HOItem.display = null;
        this.HOItem = null;
        if (this.HOText != null)
        {
            this.HOText.removeFromParent(true);
        }
        this.HOText = null;
        this.listedItemsPlace.removeFromParent(true);
        this.listedItemsPlace = null;
        this.itemsData = null;
        while (this.itemAsNormal.Count > 0)
        {
            this.itemAsNormal.pop();
        }
        while (this.itemMultiples.Count > 0)
        {
            this.itemMultiples.pop();
        }
    }

    public void init(XML arg1)
    {
        this.itemFileNameAr = new List<string>();
        this.itemNameAr = new List<string>();
        this.itemDisplayAr = new List<Sprite>();
        this.itemPosXAr = new List<float>();
        this.itemPosYAr = new List<float>();
        this.itemTypeAr = new List<int>();
        this.itemHOAr = new List<HOObjects>();
        this.itemUIAr = new List<HOObjects>();
        this.itemTextUIAr = new List<ExtendTextField>();
        this.listedItemAr = new List<int>();
        this.itemAsNormal = new List<int>();
        this.itemFoundAr = new List<int>();
        this.itemCoins = new List<int>();
        this.itemMultiples = new List<List<int>>();
        this.itemLogoIdx = -1;
        this.readItemData(arg1);
    }

    public void buildPixelHitArea(Bitmap arg1)
    {
        this.pixelHitArea = new PixelHitArea(arg1, 0.5f);
    }

    public void initHOItems(int arg1, int arg2, TextureAtlas arg3)
    {
        this.listedItemAr.Clear();
        this.loadedPack = arg1;
        this.maxItems = arg2;
        this.totalItems = arg2;
        int count = this.itemsData.children().length();
        for (int i = 0; i < count; i++)
        {
            this.HOItem = new HOObjects(arg3.getTexture(this.itemFileNameAr[i]), this.itemNameAr[i], this.itemTypeAr[i], i,
                this.itemPosXAr[i], this.itemPosYAr[i], this.pixelHitArea);
            this._global._minigameHO.zoomContainer.addChild(this.HOItem.display);
            this._global._minigameHO.zoomContainer.addChild(this.HOItem.path);
            this.itemDisplayAr.Add(this.HOItem.display);
            this.itemHOAr.Add(this.HOItem);
        }
        if (!TGlobalClass.loadGame)
        {
            for (int n = 0; n < arg2; n++)
            {
                int pick = TGlobalClass.RandomRange(0, this.itemAsNormal.Count - 1);
                int itemId = this.itemAsNormal[pick];
                this.itemAsNormal.splice(pick, 1);
                this.listedItemAr.Add(itemId);
                this.itemHOAr[itemId].listed = true;
            }
        }
        count = this.itemDisplayAr.Count;
        for (int i = 0; i < count; i++)
        {
            this.itemDisplayAr[i].addEventListener(TouchEvent.TOUCH, this.touchHOObjects);
        }
    }

    public void initUIObjects(TextureAtlas arg1)
    {
        int listedCount = this.listedItemAr.Count;
        this.listedItemsPlace = new Quad(620, 100, 16777215);
        this.listedItemsPlace.x = 220;
        this.listedItemsPlace.alpha = 0.5f;
        this.listedItemsPlace.touchable = false;
        this._global._gameS.addChild(this.listedItemsPlace);
        int placeWidth = (int)this.listedItemsPlace.width; // unused (as in AS3)
        var mergedMultiples = new List<int>();
        for (int i = 0; i < listedCount; i++)
        {
            int itemId = this.listedItemAr[i];
            int j;
            for (j = 0; j < mergedMultiples.Count; j++)
            {
                if (itemId == mergedMultiples[j]) break;
            }
            if (j != mergedMultiples.Count) continue; // already counted as part of a multiple

            string label = this.itemNameAr[itemId];
            if (this.itemTypeAr[itemId] == AS_MULTIPLE_ID)
            {
                if (this.itemHOAr[itemId].found) continue;
                int sameCount = 1;
                for (j = i + 1; j < listedCount; j++)
                {
                    if (this.itemNameAr[this.listedItemAr[j]] == label && !this.itemHOAr[this.listedItemAr[j]].found)
                    {
                        mergedMultiples.Add(this.listedItemAr[j]);
                        sameCount++;
                    }
                }
                int multipleIdx = 0;
                for (int k = 0; k < itemId; k++)
                {
                    if (this.itemTypeAr[k] == AS_MULTIPLE_ID) multipleIdx++;
                }
                string plural = this.itemsData.children().attribute("plural")[multipleIdx];
                if (sameCount == 1)
                {
                    label = label.charAt(0).toUpperCase() + label.substring(1);
                }
                else
                {
                    label = sameCount + " " + plural;
                }
            }
            this.HOText = new ExtendTextField(190, 130, label, "bromine", 25, 0);
            this.HOText.hAlign = "center";
            this.HOText.vAlign = "center";
            this.HOText.pivotX = this.HOText.width / 2;
            this.HOText.pivotY = this.HOText.height / 2;
            this.HOText.touchable = false;
            this.HOText.text = this.HOText.text.replace("_QUOTE_", "'");
            string first = this.HOText.text.charAt(0).toUpperCase();
            this.HOText.text = first + this.HOText.text.substring(1);
            string space = " ";
            for (int c = 0; c < this.HOText.text.Length; c++)
            {
                if (this.HOText.text.charCodeAt(c) == space.charCodeAt(0))
                {
                    List<string> chars = this.HOText.text.split("");
                    // AS3 would throw on a trailing space (undefined.toUpperCase()); guarded here
                    if (c + 1 < chars.Count) chars[c + 1] = chars[c + 1].toUpperCase();
                    string joined = chars.join("");
                    this.HOText.text = joined;
                }
            }
            this.HOText.text = this.HOText.text.replace(" Of ", " of ");
            this.HOText.text = this.HOText.text.replace(" And ", " and ");
            this.HOText.idx = itemId;
            this.HOText.x = this.listedItemsPlace.x;
            this.HOText.y = -300;
            this.itemTextUIAr.Add(this.HOText);
            this._global._gameS.addChild(this.HOText);
            for (j = 0; j < this.itemFoundAr.Count; j++)
            {
                if (itemId == this.itemFoundAr[j]) this.HOText.itemIsFound = true;
            }
        }
        this.setListedItems();
    }

    public void setListedItems()
    {
        int place = 0;
        int count = this.listedItemAr.Count;
        for (int i = 0; i < count; i++)
        {
            if (this.itemTextUIAr[i].itemIsFound == true) continue;
            if (place > 2) break;
            this.itemTextUIAr[i].y = this.listedItemsPlace.y + 50;
            this.itemTextUIAr[i].x = place * 200 + 235 + this.itemTextUIAr[i].width / 2;
            this.itemTextUIAr[i].aktif = true;
            this.itemTextUIAr[i].placeNumber = place;
            this.occupiedPlace[place] = true;
            place++;
        }
    }

    public void removeItem(int arg1, int arg2, bool arg3 = true)
    {
        this.itemTextUIAr[arg2].x = -100;
        this.itemTextUIAr[arg2].itemIsFound = true;
        this.occupiedPlace[this.itemTextUIAr[arg2].placeNumber] = false;
        this.itemDisplayAr[arg1].x = -200;
        this.itemHOAr[arg1].onTween = false;
        this.totalItems = this.totalItems - 1;
        this.itemTweening = false;
        this.setDisplayedItems();
        if (this.totalItems <= 0)
        {
            this.onLevelComplete();
        }
    }

    public void onLevelComplete()
    {
        int earned = 0;
        int prevBigBusiness = 0;
        var player = TSavegame.currentPlayer;
        int lvl = TGlobalClass.currentPlayedLevel;
        if (!TGlobalClass.extrasMode)
        {
            if (player.daysEarning[lvl] == 0 && lvl == player.passedDays)
            {
                if (this.itemFoundCount >= this._global._minigameHO.goalTarget)
                {
                    if (!player.unlocked[Achievement.QUALIFIED_CHEF])
                    {
                        player.achievementCounter[Achievement.QUALIFIED_CHEF] = player.achievementCounter[Achievement.QUALIFIED_CHEF] + 1;
                        if (player.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])
                        {
                            player.unlocked[Achievement.QUALIFIED_CHEF] = true;
                            Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
                        }
                        else if (player.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 == 0)
                        {
                            Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
                        }
                    }
                    player.daysEarning[lvl] = this._global._minigameHO._HOClass.itemFoundCount;
                    player.passedDays = player.passedDays + 1;
                    if (this.totalItems == 0)
                    {
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 6);
                        TGlobalClass.coinsGot = 6;
                        if (!player.unlocked[Achievement.INTERNATIONAL_CHEF])
                        {
                            player.achievementCounter[Achievement.INTERNATIONAL_CHEF] = player.achievementCounter[Achievement.INTERNATIONAL_CHEF] + 1;
                            if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])
                            {
                                player.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
                                Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
                            }
                            else if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 == 0)
                            {
                                Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
                            }
                        }
                    }
                    else
                    {
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                    }
                }
                earned = this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney);
                TSavegame.save();
            }
            else
            {
                if (this.itemFoundCount >= this._global._minigameHO.goalTarget &&
                    this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney) > player.daysEarning[lvl])
                {
                    TGlobalClass.daysEarningPrev = player.daysEarning[lvl];
                    if (this.totalItems == 0 && this.itemFoundCount > player.daysEarning[lvl])
                    {
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                        if (!player.unlocked[Achievement.INTERNATIONAL_CHEF])
                        {
                            player.achievementCounter[Achievement.INTERNATIONAL_CHEF] = player.achievementCounter[Achievement.INTERNATIONAL_CHEF] + 1;
                            if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])
                            {
                                player.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
                                Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
                            }
                            else if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 == 0)
                            {
                                Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
                            }
                        }
                    }
                    earned = this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney) - player.daysEarning[lvl];
                    player.daysEarning[lvl] = this.itemFoundCount;
                }
                TSavegame.save();
            }
            if (!player.unlocked[Achievement.BIG_BUSINESS])
            {
                prevBigBusiness = player.achievementCounter[Achievement.BIG_BUSINESS];
                player.achievementCounter[Achievement.BIG_BUSINESS] = player.achievementCounter[Achievement.BIG_BUSINESS] + earned;
                if (player.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])
                {
                    player.unlocked[Achievement.BIG_BUSINESS] = true;
                    Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
                }
                else if (prevBigBusiness % 20000 > player.achievementCounter[Achievement.BIG_BUSINESS] % 20000)
                {
                    Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
                }
            }
            TSavegame.save();
            if (this.itemFoundCount >= this._global._minigameHO.goalTarget)
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE);
            }
            else
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_FAILED);
            }
        }
        else
        {
            this._global._soundAsset.playSFX("goal_reached");
            if (this._global._minigameHO.currentLevel >= this._global._minigameHO.levelSet.Count - 1)
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE_EXTRAS_ALL);
            }
            else if (this._global._minigameHO.gameover)
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_FAILED_EXTRAS);
            }
            else
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE_EXTRAS);
            }
        }
    }

    public void removeMultipleItem(int arg1, int arg2)
    {
        this.multipleItemsTweening = this.multipleItemsTweening - 1;
        TweenLite.to(this.itemDisplayAr[arg1], 0.7f, new Vars { ["scaleX"] = 0, ["scaleY"] = 0 });
        this.itemTweening = false;
        string label = this.itemTextUIAr[arg2].text;
        int remaining = AS3.toInt(label.substring(0, label.indexOf(" ")));
        if (remaining == 0)
        {
            this.removeItem(arg1, arg2);
            return;
        }
        if (remaining > 2)
        {
            this.itemTextUIAr[arg2].text = (remaining - 1) + label.substring(label.indexOf(" "));
        }
        else
        {
            string name = this.itemNameAr[this.itemTextUIAr[arg2].idx];
            this.itemTextUIAr[arg2].text = name.charAt(0).toUpperCase() + name.substring(1);
        }
    }

    public void setDisplayedItems(int arg1 = -1)
    {
        int textCount = this.itemTextUIAr.Count;
        int placeCount = this.occupiedPlace.Count;
        for (int place = 0; place < placeCount; place++)
        {
            for (int i = 0; i < textCount; i++)
            {
                if (!(this.itemTextUIAr[i].itemIsFound == true) && !(this.itemTextUIAr[i].aktif == true) && !(this.occupiedPlace[place] == true))
                {
                    this.itemTextUIAr[i].y = this.listedItemsPlace.y + 50;
                    this.itemTextUIAr[i].x = place * 200 + 235 + this.itemTextUIAr[i].width / 2;
                    this.itemTextUIAr[i].aktif = true;
                    this.itemTextUIAr[i].placeNumber = place;
                    this.occupiedPlace[place] = true;
                    break;
                }
            }
        }
    }

    public void readItemData(XML arg1)
    {
        this.itemsData = arg1;
        int count = this.itemsData.children().length();
        XMLList items = this.itemsData.children();
        for (int i = 0; i < count; i++)
        {
            string fileName = items.attribute("name")[i];
            int len = fileName.Length;
            string tags = fileName.substring(fileName.indexOf(",") + 1, len);
            this.itemFileNameAr.Add(fileName);
            this.itemPosXAr.Add(items.attribute("locX")[i].toFloat());
            this.itemPosYAr.Add(items.attribute("locY")[i].toFloat());
            if (match(tags, "nm,mul"))
            {
                this.itemTypeAr.Add(AS_MULTIPLE_ID);
                this.itemNameAr.Add(fileName.substring(0, fileName.indexOf("_")));
                int m;
                for (m = 0; m < this.itemMultiples.Count; m++)
                {
                    if (this.itemNameAr[this.itemMultiples[m][0]] == this.itemNameAr[i])
                    {
                        this.itemMultiples[m].Add(i);
                        break;
                    }
                }
                if (m == this.itemMultiples.Count)
                {
                    var group = new List<int>();
                    group.Add(i);
                    this.itemMultiples.Add(group);
                }
                continue;
            }
            this.itemNameAr.Add(fileName.substring(0, fileName.indexOf(",")));
            if (match(tags, "nm,coin"))
            {
                this.itemTypeAr.Add(AS_COIN_ID);
                this.itemCoins.Add(i);
            }
            else if (match(tags, "nm,logo"))
            {
                this.itemTypeAr.Add(AS_LOGO_ID);
                this.itemLogoIdx = i;
            }
            else if (match(tags, "nm"))
            {
                this.itemTypeAr.Add(AS_HO_OBJECT_ID);
                this.itemAsNormal.Add(i);
                this.itemCounting = this.itemCounting + 1;
            }
            else if (match(tags, "bg2"))
            {
                this.itemTypeAr.Add(AS_BG2_ID);
            }
            else if (match(tags, "bg"))
            {
                this.itemTypeAr.Add(AS_BG_ID);
            }
        }
    }

    /// <summary>AS3 String.match(literal) != null (the patterns used here contain no regex metacharacters).</summary>
    static bool match(string s, string pattern) => s.IndexOf(pattern, StringComparison.Ordinal) >= 0;

    static string clickSound(int soundCombo) => "SFX_gameplay_click_0" + (soundCombo > 7 ? 8 : soundCombo + 1);

    public void touchHOObjects(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this.targetIDBegan = AS3.toInt((e.currentTarget as Sprite).name);
        }
        if (e.getTouch(this._global._game, TouchPhase.ENDED) == null) return;

        if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_4 && this._global.tutorial.dialogueCounter < 4) return;
        if (this.itemFoundCount >= this.maxItems || this._global._minigameHO.touchDragged) return;
        int target = AS3.toInt((e.currentTarget as Sprite).name);
        if (this.targetIDBegan != target) return;
        this.onClick = true;
        if (this.itemHOAr[target].listed == true && this.itemHOAr[target].found == false)
        {
            int textCount = this.itemTextUIAr.Count;
            for (int i = 0; i < textCount; i++)
            {
                if (this.itemTypeAr[target] == AS_MULTIPLE_ID)
                {
                    if (this.itemNameAr[target] == this.itemNameAr[this.itemTextUIAr[i].idx] && !this.itemHOAr[target].found &&
                        this.itemTextUIAr[i].aktif == true)
                    {
                        this.getCorrectMultipleItem(target, i);
                        this._global._soundAsset.playSFX(clickSound(this.soundCombo));
                        break;
                    }
                }
                else if (target == this.itemTextUIAr[i].idx && this.itemTextUIAr[i].itemIsFound == false && this.itemTextUIAr[i].aktif == true)
                {
                    this.getCorrectItem(target, i);
                    this._global._soundAsset.playSFX(clickSound(this.soundCombo));
                    if (this.revealedItem != null && target == this.revealedItem.itemIdx)
                    {
                        this.getRevealedItem = true;
                    }
                    break;
                }
                else if (target == this.itemTextUIAr[i].idx && this.itemTextUIAr[i].itemIsFound == false && this.itemTextUIAr[i].aktif == false)
                {
                    this._global._minigameHO.onMissClick();
                    this.missClickCount = this.missClickCount + 1;
                    this._global._soundAsset.playSFX("SFX_wrong_click");
                    break;
                }
            }
        }
        else if (this.itemHOAr[target].listed != true)
        {
            this._global._soundAsset.playSFX("SFX_wrong_click");
            this._global._minigameHO.onMissClick();
            this.missClickCount = this.missClickCount + 1;
        }
    }

    public void getCorrectItem(int arg1, int arg2)
    {
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "get_correct_ho_item"));
        this.addScore();
        var player = TSavegame.currentPlayer;
        if (!TGlobalClass.extrasMode && !player.unlocked[Achievement.HAWKEYE])
        {
            player.achievementCounter[Achievement.HAWKEYE] = player.achievementCounter[Achievement.HAWKEYE] + 1;
            if (player.achievementCounter[Achievement.HAWKEYE] > Achievement.ALL_TARGET[Achievement.HAWKEYE])
            {
                player.unlocked[Achievement.HAWKEYE] = true;
                Achievement.tweenAchievementNotif(Achievement.HAWKEYE);
            }
            else if (player.achievementCounter[Achievement.HAWKEYE] % 50 == 0)
            {
                Achievement.tweenAchievementProgress(Achievement.HAWKEYE);
            }
        }
        Point pos = this.itemDisplayAr[arg1].parent.localToGlobal(new Point(this.itemDisplayAr[arg1].x, this.itemDisplayAr[arg1].y));
        this._global._gameS.addChild(this.itemDisplayAr[arg1]);
        this._global._gameS.setChildIndex(this.itemDisplayAr[arg1], this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        this.itemDisplayAr[arg1].x = pos.x;
        this.itemDisplayAr[arg1].y = pos.y;
        this.itemDisplayAr[arg1].scaleX = this._global._minigameHO.zoomContainer.scaleX;
        this.itemDisplayAr[arg1].scaleY = this._global._minigameHO.zoomContainer.scaleY;
        this.itemHOAr[arg1].found = true;
        this.itemFoundAr.Add(arg1);
        this.missClickCount = 0;
        this.itemTextUIAr[arg2].itemIsFound = true;
        this.itemFoundCount = this.itemFoundCount + 1;
        this.comboCount = this.comboCount + 1;
        this.soundCombo = this.soundCombo + 1;
        if (!this._global._minigameHO.hasHint)
        {
            this._global._minigameHO.hintTimer = this._global._minigameHO.hintTimer + 1;
        }
        this.tweenFoundItem(arg1, arg2, 0);
    }

    public void getCorrectMultipleItem(int arg1, int arg2)
    {
        this.addScore();
        string label = this.itemTextUIAr[arg2].text;
        int remaining = AS3.toInt(label.substring(0, label.indexOf(" ")));
        remaining = remaining - this.multipleItemsTweening;
        if (remaining <= 1)
        {
            TSavegame.currentPlayer.totalItemsClassic = TSavegame.currentPlayer.totalItemsClassic - 1;
        }
        this.multipleItemsTweening = this.multipleItemsTweening + 1;
        Point pos = this.itemDisplayAr[arg1].parent.localToGlobal(new Point(this.itemDisplayAr[arg1].x, this.itemDisplayAr[arg1].y));
        this._global._gameS.addChild(this.itemDisplayAr[arg1]);
        this._global._gameS.setChildIndex(this.itemDisplayAr[arg1], this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        this.itemDisplayAr[arg1].x = pos.x;
        this.itemDisplayAr[arg1].y = pos.y;
        this.itemDisplayAr[arg1].scaleX = this._global._minigameHO.zoomContainer.scaleX;
        this.itemDisplayAr[arg1].scaleY = this._global._minigameHO.zoomContainer.scaleY;
        this.itemHOAr[arg1].found = true;
        this.itemFoundAr.Add(arg1);
        this.itemFoundCount = this.itemFoundCount + 1;
        this.missClickCount = 0;
        this.comboCount = this.comboCount + 1;
        this.soundCombo = this.soundCombo + 1;
        if (!this._global._minigameHO.hasHint)
        {
            this._global._minigameHO.hintTimer = this._global._minigameHO.hintTimer + 1;
        }
        this.tweenFoundItem(arg1, arg2, 0);
    }

    public void tweenFoundItem(int arg1, int arg2, int arg3)
    {
        int targetID = arg1;
        int UIIdx = arg2;
        int tweenCount = arg3;
        this.itemHOAr[targetID].onTween = true;
        this.itemTweening = true;
        TweenLite.killTweensOf(this.itemDisplayAr[targetID]);
        if (tweenCount < 1)
        {
            tweenCount = tweenCount + 1;
            TweenLite.to(this.itemDisplayAr[targetID], 0.4f, new Vars
            {
                ["scaleX"] = this.itemDisplayAr[targetID].scaleX + 0.4f,
                ["scaleY"] = this.itemDisplayAr[targetID].scaleY + 0.4f,
                ["ease"] = Elastic.easeOut,
                ["onComplete"] = (Action<int, int, int>)this.tweenFoundItem,
                ["onCompleteParams"] = new object[] { targetID, UIIdx, tweenCount },
            });
            if (targetID == this.revealedIdx)
            {
                this._global.ps.pause();
                this._global.ps.removeEventListener(Event.ENTER_FRAME, this.tweenCircle);
                this.currentAngle = 0.25f;
            }
            if (TGlobalClass.EnableFoundItemParticle)
            {
                this._global.psFound.start();
                this._global._gameS.setChildIndex(this._global.psFound, this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
                this._global.psFound.emitterX = this.itemDisplayAr[targetID].x;
                this._global.psFound.emitterY = this.itemDisplayAr[targetID].y;
                TweenLite.delayedCall(0.5f, (Action)(() =>
                {
                    this._global.psFound.pause();
                }));
            }
            return;
        }

        float randomFirstX = TGlobalClass.RandomRange(1, 2);
        if (randomFirstX == 1)
        {
            randomFirstX = -100;
        }
        else
        {
            randomFirstX = 100;
        }
        float randomSecondX = TGlobalClass.RandomRange(30, 80);
        if (!(randomFirstX > 0))
        {
            randomSecondX = -randomSecondX;
        }
        Sprite item = this.itemDisplayAr[targetID];
        ExtendTextField ui = this.itemTextUIAr[UIIdx];
        float selisih = item.y - ui.y;
        var points = new List<object>
        {
            new Dictionary<string, object> { ["x"] = item.x, ["y"] = item.y },
            new Dictionary<string, object> { ["x"] = item.x + randomFirstX, ["y"] = item.y + selisih / 3 },
            new Dictionary<string, object> { ["x"] = item.x + randomSecondX, ["y"] = item.y - selisih / 3 * 4 },
            new Dictionary<string, object> { ["x"] = ui.x, ["y"] = ui.y },
        };
        this.itemHOAr[targetID].path.follower = item;
        this.itemHOAr[targetID].path.init(points);
        float jarak = Mathf.Sqrt(Mathf.Pow(item.y - ui.y, 2) + Mathf.Pow(item.x - ui.x, 2));
        float multiplier = jarak > 500 ? 0.001f : (jarak < 200 ? 0.0025f : 0.0015f);
        this.itemHOAr[targetID].path.start(0.3f + jarak * multiplier, targetID, UIIdx, Linear.easeNone);
        TweenLite.to(item, 0.3f + jarak * multiplier, new Vars
        {
            ["scaleX"] = ui.scaleX,
            ["scaleY"] = ui.scaleY,
            ["rotation"] = 360 * Mathf.PI / 180,
            ["ease"] = Linear.easeNone,
        });
    }

    public void revealItem()
    {
        if (!this._global._minigameHO.hasHint) return;
        var available = new List<int>();
        for (int i = 0; i < this.itemTextUIAr.Count; i++)
        {
            ExtendTextField ui = this.itemTextUIAr[i];
            if (this.itemTypeAr[ui.idx] == AS_MULTIPLE_ID)
            {
                if (ui.aktif == true && ui.itemIsFound == false)
                {
                    for (int j = 0; j < this.listedItemAr.Count; j++)
                    {
                        if (this.itemNameAr[ui.idx] == this.itemNameAr[this.listedItemAr[j]] && !this.itemHOAr[this.listedItemAr[j]].found)
                        {
                            available.Add(this.listedItemAr[j]);
                        }
                    }
                }
            }
            else if (ui.aktif == true && ui.itemIsFound == false)
            {
                available.Add(ui.idx);
            }
        }
        if (available.Count == 0) return;
        this._global._soundAsset.playSFX("bell_dring");
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "used_hint"));
        if (this._global._minigameHO.zoomContainer.scaleX > 1)
        {
            this.onScaleBack = true;
            TweenLite.to(this._global._minigameHO.zoomContainer, 0.2f, new Vars
            {
                ["scaleX"] = 1,
                ["scaleY"] = 1,
                ["x"] = this._global._minigameHO.zoomContainer.pivotX,
                ["y"] = this._global._minigameHO.zoomContainer.pivotY,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() =>
                {
                    this.onScaleBack = false;
                }),
            });
        }
        this.revealedIdx = available[TGlobalClass.RandomRange(0, available.Count - 1)];
        this.revealedItem = this.itemHOAr[this.revealedIdx];
        this._global._minigameHO.useHint();
        if (TGlobalClass.EnableHintParticle)
        {
            this._global.ps.start();
            var bPath = new BezierPath();
            float X = this.itemDisplayAr[this.revealedIdx].x;
            float Y = this.itemDisplayAr[this.revealedIdx].y;
            this._global.ps.emitterX = HINT_X;
            this._global.ps.emitterY = HINT_Y;
            this.radiusX = this.itemDisplayAr[this.revealedIdx].width;
            this.radiusY = this.itemDisplayAr[this.revealedIdx].height;
            bPath.follower = this._global.ps;
            bPath.init(new List<object>
            {
                new Dictionary<string, object> { ["x"] = HINT_X, ["y"] = HINT_Y },
                new Dictionary<string, object> { ["x"] = HINT_X, ["y"] = HINT_Y - TGlobalClass.RandomRange(-100, 300) },
                new Dictionary<string, object> { ["x"] = X - TGlobalClass.RandomRange(200, 400), ["y"] = Y + this.radiusY },
                new Dictionary<string, object> { ["x"] = X, ["y"] = Y + this.radiusY },
            });
            float jarak = Mathf.Sqrt(Mathf.Pow(X - HINT_X, 2) + Mathf.Pow(Y - HINT_Y, 2));
            bPath.startPS(0.003f * jarak, Linear.easeNone);
            this.centerX = X;
            this.centerY = Y;
        }
        GALib.TrackEvent(GALib.EVENT_PRESS_HINT, this._global._minigameHO.currentLevel + 1, this.itemNameAr[this.revealedIdx]);
    }

    public void tweenCircle(Event arg1)
    {
        if (this._global._minigameHO.paused) return;
        this.currentAngle = this.currentAngle - this.angleStep;
        this._global.ps.emitterX = this.centerX + Mathf.Cos(this.currentAngle * this.twoPI) * this.radiusX;
        this._global.ps.emitterY = this.centerY + Mathf.Sin(this.currentAngle * this.twoPI) * this.radiusY;
        if (this.currentAngle < -1.75f)
        {
            this.currentAngle = 0.25f;
            this._global.ps.pause();
            this._global.ps.removeEventListener(Event.ENTER_FRAME, this.tweenCircle);
        }
    }

    public void addScore()
    {
        this._global._minigameHO.updateMoney();
    }
}
