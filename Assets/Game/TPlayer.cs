// Ported from TPlayer.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class TPlayer
{
    public string name;
    public int playerIdx;
    public List<int> ingredientLevel;
    public List<int> chineseDishLevel;
    public List<int> westernDishLevel;
    public List<int> mideastDishLevel;
    public List<int> hardwareLevel;
    public int passedDays;
    public List<int> daysEarning;
    public List<bool> goalAchieved;
    public List<bool> expertAchieved;
    public string money;
    public int lastCustIdx = 2;
    public int lastDayKitchen = 0;
    public bool openingViewed = false;
    public bool endingViewed = false;
    public int totalItemsClassic;
    public List<bool> unlocked;
    public List<int> achievementCounter;
    public List<bool> tutorialShowed;

    public TPlayer()
    {
        this.ingredientLevel = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        this.chineseDishLevel = new List<int> { 0, -1, -1, -1, -1, -1 };
        this.westernDishLevel = new List<int> { -1, -1, -1, -1, -1, -1 };
        this.mideastDishLevel = new List<int> { -1, -1, -1, -1, -1, -1 };
        this.hardwareLevel = new List<int> { 0, 0, 0 };
        this.daysEarning = AS3Extensions.filled<int>(TGlobalClass.TOTAL_DAYS, i => 0);
        this.goalAchieved = AS3Extensions.filled<bool>(TGlobalClass.TOTAL_DAYS, i => false);
        this.expertAchieved = AS3Extensions.filled<bool>(TGlobalClass.TOTAL_DAYS, i => false);
        this.unlocked = new List<bool> { false, false, false, false, false, false, false, false, false, false };
        this.achievementCounter = new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        this.tutorialShowed = AS3Extensions.filled<bool>(Tutorial.TUTORIAL_COUNT, i => false);
    }
}
