// Ported from Dish.as (decompiled). Reference example for the porting conventions.
using System.Collections.Generic;
using Starlite;

public class Dish
{
    public const int VEGGIE_CROISSANT = 1;
    public const int JUICER = 0;
    public const int FRYER = 1;
    public const int COOKIES = 2;
    public XML dishXML;

    public Dish()
    {
    }

    public void init()
    {
        var request = new URLRequest("data/Dish.xml");
        var loader = new URLLoader();
        loader.dataFormat = URLLoaderDataFormat.TEXT;
        loader.addEventListener(Event.COMPLETE, this.dishLoadComplete);
        loader.load(request);
        TGlobalClass.dishRegionWestern = new List<int>();
        TGlobalClass.dishRegionChinese = new List<int>();
        TGlobalClass.dishRegionMidEast = new List<int>();
        TGlobalClass.westernUpgradeCost = AS3Extensions.filled<List<int>>(ConstClass.WESTERN_DISH_COUNT, i => null);
        TGlobalClass.chineseUpgradeCost = AS3Extensions.filled<List<int>>(ConstClass.CHINESE_DISH_COUNT, i => null);
        TGlobalClass.mideastUpgradeCost = AS3Extensions.filled<List<int>>(ConstClass.MID_EAST_DISH_COUNT, i => null);
        TGlobalClass.ingredientCost = AS3Extensions.filled<List<int>>(ConstClass.INGREDIENT_COUNT, i => null);
    }

    public void dishLoadComplete(Event arg1)
    {
        int regionIdx = 0;
        this.dishXML = new XML((string)((URLLoader)arg1.target).data);
        for (int i = 0; i < this.dishXML["dish"].length(); i++)
        {
            string region = this.dishXML["dish"][i].attribute("region");
            if (region == "western") TGlobalClass.dishRegionWestern.Add(i);
            else if (region == "chinese") TGlobalClass.dishRegionChinese.Add(i);
            else TGlobalClass.dishRegionMidEast.Add(i);

            List<string> costs = ((string)this.dishXML["dish"][i].attribute("upgradeCost")).split(",");
            if (region == "western")
            {
                TGlobalClass.westernUpgradeCost[regionIdx] = new List<int>();
                for (int k = 0; k < costs.Count; k++) TGlobalClass.westernUpgradeCost[regionIdx].Add(AS3.toInt(costs[k]));
                regionIdx++;
                if (regionIdx == ConstClass.WESTERN_DISH_COUNT) regionIdx = 0;
            }
            else if (region == "mid_east")
            {
                TGlobalClass.mideastUpgradeCost[regionIdx] = new List<int>();
                for (int k = 0; k < costs.Count; k++) TGlobalClass.mideastUpgradeCost[regionIdx].Add(AS3.toInt(costs[k]));
                regionIdx++;
                if (regionIdx == ConstClass.MID_EAST_DISH_COUNT) regionIdx = 0;
            }
            else if (region == "chinese")
            {
                TGlobalClass.chineseUpgradeCost[regionIdx] = new List<int>();
                for (int k = 0; k < costs.Count; k++) TGlobalClass.chineseUpgradeCost[regionIdx].Add(AS3.toInt(costs[k]));
                regionIdx++;
                if (regionIdx == ConstClass.CHINESE_DISH_COUNT) regionIdx = 0;
            }
        }
        for (int i = 0; i < this.dishXML["ingredient"].length(); i++)
        {
            TGlobalClass.ingredientCost[i] = new List<int>();
            List<string> costs = ((string)this.dishXML["ingredient"][i].attribute("upgradeCost")).split(",");
            for (int k = 0; k < costs.Count; k++) TGlobalClass.ingredientCost[i].Add(AS3.toInt(costs[k]));
        }
    }

    static List<int> IntList(string csv)
    {
        var r = new List<int>();
        foreach (var s in csv.split(",")) r.Add(AS3.toInt(s));
        return r;
    }

    public List<int> getDishIngredientType(int arg1) => IntList(this.dishXML["dish"][arg1].attribute("ingredient"));
    public List<int> getDishTarget(int arg1) => IntList(this.dishXML["dish"][arg1].attribute("defaultTarget"));
    public int getDishDefaultPrice(int arg1) => this.dishXML["dish"][arg1].attribute("defaultPrice").toInt();
    public int getPriceIncrease(int arg1) => this.dishXML["dish"][arg1].attribute("priceIncrease").toInt();
    public int getIngredientPriceIncrease(int arg1) => this.dishXML["ingredient"][arg1].attribute("priceIncrease").toInt();
    public List<int> getHardwareUpgradecost(int arg1) => IntList(this.dishXML["hardware"][arg1].attribute("upgradeCost"));
    public int getHardwareDefaultPrice(int arg1) => this.dishXML["hardware"][arg1].attribute("defaultPrice").toInt();
    public int getHardwarePriceIncrease(int arg1) => this.dishXML["hardware"][arg1].attribute("priceIncrease").toInt();
    public int getHardwareDefaultTarget(int arg1) => this.dishXML["hardware"][arg1].attribute("startingTarget").toInt();
    public int getHardwareReduction(int arg1) => this.dishXML["hardware"][arg1].attribute("Reduction").toInt();
}
