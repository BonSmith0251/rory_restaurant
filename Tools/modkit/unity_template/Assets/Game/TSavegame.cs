// Ported from TSavegame.as (decompiled). Static save system over the SharedObjects
// "Match3Cooking" (player slots) and "Match3CookingSetting" (mute flags).
// Starlite SharedObject data is a Dictionary persisted as JSON; values read back are
// int/long/double/bool/string/List<object>/Dictionary, so the typed lists are rebuilt here.
using System;
using System.Collections.Generic;
using Starlite;

public class TSavegame
{
    public static bool fullScreen;
    public static List<bool> usedSlot;
    public static List<string> playerName;
    public static List<List<int>> ingredientLevel;
    public static List<List<int>> westernLevel;
    public static List<List<int>> chineseLevel;
    public static List<List<int>> mideastLevel;
    public static List<List<int>> hardwareLevel;
    public static List<string> money;
    public static List<int> lastCustIdx;
    public static List<int> lastKitchenDay;
    public static List<bool> openingViewed;
    public static List<bool> endingViewed;
    public static List<int> passedDays;
    public static List<List<int>> daysEarning;
    public static List<List<bool>> goalAchieved;
    public static List<List<bool>> expertAchieved;
    public static List<List<bool>> unlocked;
    public static List<List<int>> achievementCounter;
    public static List<List<bool>> tutorialShowed;
    public static SharedObject _saveGame;
    public static SharedObject _saveGameSetting;
    public static int lastPlayer;
    public static bool muteMusic;
    public static bool muteSound;
    public static TPlayer currentPlayer;

    static TSavegame()
    {
        usedSlot = AS3Extensions.filled<bool>(5, i => false);
        playerName = AS3Extensions.filled<string>(5, i => null);
        ingredientLevel = AS3Extensions.filled<List<int>>(5, i => null);
        westernLevel = AS3Extensions.filled<List<int>>(5, i => null);
        chineseLevel = AS3Extensions.filled<List<int>>(5, i => null);
        mideastLevel = AS3Extensions.filled<List<int>>(5, i => null);
        hardwareLevel = AS3Extensions.filled<List<int>>(5, i => null);
        money = AS3Extensions.filled<string>(5, i => null);
        lastCustIdx = AS3Extensions.filled<int>(5, i => 0);
        lastKitchenDay = AS3Extensions.filled<int>(5, i => 0);
        openingViewed = AS3Extensions.filled<bool>(5, i => false);
        endingViewed = AS3Extensions.filled<bool>(5, i => false);
        passedDays = AS3Extensions.filled<int>(5, i => 0);
        daysEarning = AS3Extensions.filled<List<int>>(5, i => null);
        goalAchieved = AS3Extensions.filled<List<bool>>(5, i => null);
        expertAchieved = AS3Extensions.filled<List<bool>>(5, i => null);
        unlocked = AS3Extensions.filled<List<bool>>(5, i => null);
        achievementCounter = AS3Extensions.filled<List<int>>(5, i => null);
        tutorialShowed = AS3Extensions.filled<List<bool>>(5, i => null);
        _saveGame = SharedObject.getLocal("Match3Cooking");
        _saveGameSetting = SharedObject.getLocal("Match3CookingSetting");
    }

    public TSavegame()
    {
    }

    public static void init()
    {
        currentPlayer = new TPlayer();
        lastPlayer = 0;
        muteMusic = false;
        muteSound = false;
        fullScreen = true;
        for (int i = 0; i < 5; i++)
        {
            usedSlot[i] = false;
            playerName[i] = "";
            resetSlotProgress(i);
        }
    }

    /// <summary>Per-slot defaults shared by init() and clear() (identical code in the AS3, from money on).</summary>
    static void resetSlotProgress(int i)
    {
        money[i] = "a";
        lastCustIdx[i] = 2;
        lastKitchenDay[i] = 0;
        openingViewed[i] = false;
        endingViewed[i] = false;
        ingredientLevel[i] = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        westernLevel[i] = new List<int> { 1, 0, 0, 0, 0, 0 };
        chineseLevel[i] = new List<int> { 0, 0, 0, 0, 0, 0 };
        mideastLevel[i] = new List<int> { 0, 0, 0, 0, 0, 0 };
        hardwareLevel[i] = new List<int> { 0, 0, 0 };
        passedDays[i] = 0;
        daysEarning[i] = AS3Extensions.filled<int>(TGlobalClass.TOTAL_DAYS, k => 0);
        goalAchieved[i] = AS3Extensions.filled<bool>(TGlobalClass.TOTAL_DAYS, k => false);
        expertAchieved[i] = AS3Extensions.filled<bool>(TGlobalClass.TOTAL_DAYS, k => false);
        unlocked[i] = new List<bool> { false, false, false, false, false, false, false, false, false, false };
        achievementCounter[i] = new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        tutorialShowed[i] = AS3Extensions.filled<bool>(Tutorial.TUTORIAL_COUNT, k => false);
    }

    public static void loadAll()
    {
        int i;
        int j;
        var d = _saveGame.data;
        if (!(!d.ContainsKey("lastPlayer") || d["lastPlayer"] == null))
        {
            usedSlot = toBoolList(get(d, "usedSlot")) ?? usedSlot;
            playerName = toStringList(get(d, "playerName")) ?? playerName;
            lastPlayer = AS3.toInt(d["lastPlayer"]);
            if (!(!d.ContainsKey("fullScreen") || d["fullScreen"] == null))
            {
                fullScreen = AS3.toBool(d["fullScreen"]);
            }
            for (i = 0; i < 5; i++)
            {
                object sWestern = at(get(d, "westernLevel"), i);
                object sChinese = at(get(d, "chineseLevel"), i);
                object sMideast = at(get(d, "mideastLevel"), i);
                for (j = 0; j < len(sWestern); j++)
                {
                    if (j < TGlobalClass.TOTAL_CHINESE_DISH) chineseLevel[i].splice(j, 1, AS3.toInt(at(sChinese, j)));
                    else chineseLevel[i].push(AS3.toInt(at(sChinese, j)));
                    if (j < TGlobalClass.TOTAL_WESTERN_DISH) westernLevel[i].splice(j, 1, AS3.toInt(at(sWestern, j)));
                    else westernLevel[i].push(AS3.toInt(at(sWestern, j)));
                    if (j < TGlobalClass.TOTAL_MIDEAST_DISH) mideastLevel[i].splice(j, 1, AS3.toInt(at(sMideast, j)));
                    else mideastLevel[i].push(AS3.toInt(at(sMideast, j)));
                }
                object sIngredient = at(get(d, "ingredientLevel"), i);
                for (j = 0; j < len(sIngredient); j++)
                {
                    if (j < TGlobalClass.TOTAL_INGREDIENT) ingredientLevel[i].splice(j, 1, AS3.toInt(at(sIngredient, j)));
                    else ingredientLevel[i].push(AS3.toInt(at(sIngredient, j)));
                }
                object sHardware = at(get(d, "hardwareLevel"), i);
                for (j = 0; j < len(sHardware); j++)
                {
                    if (j < TGlobalClass.TOTAL_HARDWARE) hardwareLevel[i].splice(j, 1, AS3.toInt(at(sHardware, j)));
                    else hardwareLevel[i].push(AS3.toInt(at(sHardware, j)));
                }
                object sDays = at(get(d, "daysEarning"), i);
                object sGoal = at(get(d, "goalAchieved"), i);
                object sExpert = at(get(d, "expertAchieved"), i);
                for (j = 0; j < len(sDays); j++)
                {
                    if (j < TGlobalClass.TOTAL_DAYS)
                    {
                        daysEarning[i].splice(j, 1, AS3.toInt(at(sDays, j)));
                        goalAchieved[i].splice(j, 1, AS3.toBool(at(sGoal, j)));
                        expertAchieved[i].splice(j, 1, AS3.toBool(at(sExpert, j)));
                    }
                    else
                    {
                        daysEarning[i].push(AS3.toInt(at(sDays, j)));
                        goalAchieved[i].push(AS3.toBool(at(sGoal, j)));
                        expertAchieved[i].push(AS3.toBool(at(sExpert, j)));
                    }
                }
            }
            passedDays = toIntList(get(d, "passedDays")) ?? passedDays;
            // Old saves stored money as plain ints: convert them to the encrypted string format once.
            if (isInt(at(get(d, "money"), 0)))
            {
                var converted = AS3Extensions.filled<string>(5, k => null);
                for (i = 0; i < 5; i++)
                {
                    converted[i] = Global.getInstance()._maxCrypto.ITS(AS3.toInt(at(get(d, "money"), i)));
                }
                d["money"] = converted;
                _saveGame.flush();
            }
            money = toStringList(get(d, "money")) ?? money;
            lastCustIdx = toIntList(get(d, "lastCustIdx")) ?? lastCustIdx;
            lastKitchenDay = toIntList(get(d, "lastKitchenDay")) ?? lastKitchenDay;
            openingViewed = toBoolList(get(d, "openingViewed")) ?? openingViewed;
            endingViewed = toBoolList(get(d, "endingViewed")) ?? endingViewed;
            for (i = 0; i < 5; i++)
            {
                object sCounter = at(get(d, "achievementCounter"), i);
                if (AS3.toBool(sCounter))
                {
                    achievementCounter[i] = toIntList(sCounter);
                }
                object sUnlocked = at(get(d, "unlocked"), i);
                if (AS3.toBool(sUnlocked))
                {
                    unlocked[i] = toBoolList(sUnlocked);
                }
            }
            for (i = 0; i < 5; i++)
            {
                object sTutorial = at(get(d, "tutorialShowed"), i);
                for (j = 0; j < len(sTutorial); j++)
                {
                    if (j < Tutorial.TUTORIAL_COUNT) tutorialShowed[i].splice(j, 1, AS3.toBool(at(sTutorial, j)));
                    else tutorialShowed[i].pop();
                }
            }
            if (lastPlayer > 0)
            {
                loadPlayer(lastPlayer - 1);
            }
        }
        muteMusic = AS3.toBool(get(_saveGameSetting.data, "muteMusic"));
        muteSound = AS3.toBool(get(_saveGameSetting.data, "muteSound"));
    }

    public static void saveAll()
    {
        var d = _saveGame.data;
        d["usedSlot"] = usedSlot.slice();
        d["playerName"] = playerName.slice();
        d["ingredientLevel"] = ingredientLevel.slice();
        d["westernLevel"] = westernLevel.slice();
        d["chineseLevel"] = chineseLevel.slice();
        d["mideastLevel"] = mideastLevel.slice();
        d["hardwareLevel"] = hardwareLevel.slice();
        d["money"] = money.slice();
        d["lastCustIdx"] = lastCustIdx.slice();
        d["lastKitchenDay"] = lastKitchenDay.slice();
        d["openingViewed"] = openingViewed.slice();
        d["endingViewed"] = endingViewed.slice();
        d["passedDays"] = passedDays.slice();
        d["daysEarning"] = daysEarning.slice();
        d["goalAchieved"] = goalAchieved.slice();
        d["expertAchieved"] = expertAchieved.slice();
        d["unlocked"] = unlocked.slice();
        d["achievementCounter"] = achievementCounter.slice();
        d["tutorialShowed"] = tutorialShowed.slice();
        d["lastPlayer"] = lastPlayer;
        d["fullScreen"] = fullScreen;
        // Flash wrote SharedObjects to disk automatically (on exit / GC); Starlite only writes on flush().
        _saveGame.flush();
    }

    public static void saveSetting()
    {
        _saveGameSetting.data["muteMusic"] = Global.getInstance().MuteMusic;
        _saveGameSetting.data["muteSound"] = Global.getInstance().MuteSound;
        _saveGameSetting.flush();
    }

    public static void save(int arg1 = -1)
    {
        if (arg1 == -1)
        {
            arg1 = currentPlayer.playerIdx;
        }
        playerName[arg1] = currentPlayer.name;
        ingredientLevel[arg1] = currentPlayer.ingredientLevel;
        westernLevel[arg1] = currentPlayer.westernDishLevel;
        chineseLevel[arg1] = currentPlayer.chineseDishLevel;
        mideastLevel[arg1] = currentPlayer.mideastDishLevel;
        hardwareLevel[arg1] = currentPlayer.hardwareLevel;
        passedDays[arg1] = currentPlayer.passedDays;
        daysEarning[arg1] = currentPlayer.daysEarning;
        goalAchieved[arg1] = currentPlayer.goalAchieved;
        expertAchieved[arg1] = currentPlayer.expertAchieved;
        money[arg1] = currentPlayer.money;
        lastCustIdx[arg1] = currentPlayer.lastCustIdx;
        lastKitchenDay[arg1] = currentPlayer.lastDayKitchen;
        openingViewed[arg1] = currentPlayer.openingViewed;
        endingViewed[arg1] = currentPlayer.endingViewed;
        unlocked[arg1] = currentPlayer.unlocked.slice();
        achievementCounter[arg1] = currentPlayer.achievementCounter.slice();
        tutorialShowed[arg1] = currentPlayer.tutorialShowed.slice();
        saveAll();
    }

    public static void loadPlayer(int arg1)
    {
        currentPlayer.playerIdx = arg1;
        currentPlayer.name = playerName[arg1];
        currentPlayer.ingredientLevel = ingredientLevel[arg1];
        currentPlayer.westernDishLevel = westernLevel[arg1];
        currentPlayer.chineseDishLevel = chineseLevel[arg1];
        currentPlayer.mideastDishLevel = mideastLevel[arg1];
        currentPlayer.hardwareLevel = hardwareLevel[arg1];
        currentPlayer.passedDays = passedDays[arg1];
        currentPlayer.daysEarning = daysEarning[arg1];
        currentPlayer.goalAchieved = goalAchieved[arg1];
        currentPlayer.expertAchieved = expertAchieved[arg1];
        currentPlayer.money = money[arg1];
        currentPlayer.lastCustIdx = lastCustIdx[arg1];
        currentPlayer.lastDayKitchen = lastKitchenDay[arg1];
        currentPlayer.openingViewed = openingViewed[arg1];
        currentPlayer.endingViewed = endingViewed[arg1];
        currentPlayer.achievementCounter = achievementCounter[arg1].slice();
        currentPlayer.unlocked = unlocked[arg1].slice();
        currentPlayer.tutorialShowed = tutorialShowed[arg1].slice();
        lastPlayer = arg1 + 1;
        save(arg1);
    }

    public static void clear(int arg1)
    {
        usedSlot[arg1] = false;
        playerName[arg1] = "";
        if (arg1 == lastPlayer - 1)
        {
            lastPlayer = 0;
        }
        resetSlotProgress(arg1);
        saveAll();
    }

    public static void saveAchievement()
    {
        achievementCounter[currentPlayer.playerIdx] = currentPlayer.achievementCounter.slice();
        unlocked[currentPlayer.playerIdx] = currentPlayer.unlocked.slice();
        _saveGame.data["_achievementCounter"] = achievementCounter.slice(); // sic: AS3 key has a leading underscore
        _saveGame.data["unlocked"] = unlocked.slice();
    }

    // ---------------------------------------------------------------- JSON value helpers

    /// <summary>data.key (null when missing, like AS3 undefined).</summary>
    static object get(Dictionary<string, object> d, string key) => d.TryGetValue(key, out var v) ? v : null;

    /// <summary>Views any stored list (JSON List&lt;object&gt; or a live typed list put by saveAll) as List&lt;object&gt;.</summary>
    static List<object> asList(object o)
    {
        switch (o)
        {
            case null: return null;
            case List<object> lo: return lo;
            case IEnumerable<object> e: return new List<object>(e);
            case List<int> li: { var r = new List<object>(li.Count); foreach (var v in li) r.Add(v); return r; }
            case List<bool> lb: { var r = new List<object>(lb.Count); foreach (var v in lb) r.Add(v); return r; }
            case List<float> lf: { var r = new List<object>(lf.Count); foreach (var v in lf) r.Add(v); return r; }
            case List<double> ld: { var r = new List<object>(ld.Count); foreach (var v in ld) r.Add(v); return r; }
            default: return null;
        }
    }

    /// <summary>list[i] with AS3 semantics (undefined -> null).</summary>
    static object at(object list, int i)
    {
        var l = asList(list);
        return l == null ? null : l.at(i);
    }

    static int len(object list) => asList(list)?.Count ?? 0;

    static bool isInt(object o) => o is int || o is long || (o is double dv && dv == Math.Floor(dv)) || (o is float fv && fv == Math.Floor(fv));

    /// <summary>Vector.&lt;Boolean&gt;(array)</summary>
    static List<bool> toBoolList(object o)
    {
        var l = asList(o);
        if (l == null) return null;
        var r = new List<bool>(l.Count);
        foreach (var v in l) r.Add(AS3.toBool(v));
        return r;
    }

    /// <summary>Vector.&lt;int&gt;(array)</summary>
    static List<int> toIntList(object o)
    {
        var l = asList(o);
        if (l == null) return null;
        var r = new List<int>(l.Count);
        foreach (var v in l) r.Add(AS3.toInt(v));
        return r;
    }

    /// <summary>Vector.&lt;String&gt;(array)</summary>
    static List<string> toStringList(object o)
    {
        var l = asList(o);
        if (l == null) return null;
        var r = new List<string>(l.Count);
        foreach (var v in l) r.Add(v == null ? null : AS3.str(v));
        return r;
    }
}
