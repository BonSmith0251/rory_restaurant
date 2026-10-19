using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Rory.Data;
using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>Converts the original XML design data (Data/Source) into ScriptableObjects (Data/*.asset).</summary>
    public static class GameDataImporter
    {
        [MenuItem("Rory/Import/Game Data (XML > ScriptableObjects)")]
        public static void ImportAll()
        {
            var db = RoryAssets.LoadOrCreate<GameDatabase>(RoryPaths.Resources + "/GameDatabase.asset");
            db.levels = ImportLevels();
            db.dishes = ImportDishes();
            db.customers = ImportCustomers();
            db.levelEvents = ImportLevelEvents();
            db.dialogues = ImportDialogues();
            db.desserts = ImportDesserts();
            db.hiddenObjectLayouts = ImportHiddenObjects();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Rory] Game data imported: {db.levels.days.Count} days, {db.dishes.dishes.Count} dishes, " +
                      $"{db.customers.customers.Count} customers, {db.dialogues.dialogues.Count} dialogues, " +
                      $"{db.hiddenObjectLayouts.Count} hidden object scenes");
        }

        static XElement Load(string file) => Parse(Path.Combine(RoryPaths.DataSource, file));

        /// <summary>The original files contain comments that strict XML rejects ("&lt;!-- ---- --&gt;"), so drop comments first.</summary>
        static XElement Parse(string path) =>
            XDocument.Parse(Regex.Replace(File.ReadAllText(path), "<!--.*?-->", "", RegexOptions.Singleline)).Root;

        static string Attr(XElement e, string name, string def = "") => (string)e.Attribute(name) ?? def;

        static float F(string s, float def = 0) =>
            float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : def;

        static int I(string s, int def = 0) => string.IsNullOrWhiteSpace(s) ? def : Mathf.RoundToInt(F(s, def));

        static List<int> Csv(string s) =>
            string.IsNullOrWhiteSpace(s) ? new List<int>() : s.Split(',').Select(x => I(x.Trim())).ToList();

        static string Child(XElement e, string name) => ((string)e.Element(name) ?? "").Trim();

        static T Save<T>(T so, string name) where T : ScriptableObject
        {
            var path = $"{RoryPaths.Data}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(so, existing);
                EditorUtility.SetDirty(existing);
                UnityEngine.Object.DestroyImmediate(so);
                return existing;
            }
            RoryAssets.EnsureFolder(RoryPaths.Data);
            AssetDatabase.CreateAsset(so, path);
            return so;
        }

        static LevelDatabase ImportLevels()
        {
            var root = Load("LevelSet.xml");
            var db = ScriptableObject.CreateInstance<LevelDatabase>();
            foreach (var d in root.Elements("day"))
            {
                db.days.Add(new DayData
                {
                    gameType = GameTypes.Parse(Child(d, "gameType")),
                    time = F(Child(d, "time")),
                    goal = I(Child(d, "goal")),
                    expert = I(Child(d, "expert")),
                    region = Child(d, "region"),
                });
            }
            foreach (var d in root.Elements("dessert"))
                db.dessertLevels.Add(new DessertLevelData { goal = I(Child(d, "goal")), time = F(Child(d, "time")) });
            return Save(db, "LevelDatabase");
        }

        static DishDatabase ImportDishes()
        {
            var root = Load("Dish.xml");
            var db = ScriptableObject.CreateInstance<DishDatabase>();
            foreach (var d in root.Elements("dish"))
            {
                db.dishes.Add(new DishData
                {
                    region = Attr(d, "region"),
                    dishName = Attr(d, "dishName"),
                    defaultPrice = I(Attr(d, "defaultPrice")),
                    priceIncrease = I(Attr(d, "priceIncrease")),
                    defaultTarget = Csv(Attr(d, "defaultTarget")),
                    ingredients = Csv(Attr(d, "ingredient")),
                    upgradeCost = Csv(Attr(d, "upgradeCost")),
                    expertNeeded = I(Attr(d, "expertNeeded")),
                });
            }
            foreach (var d in root.Elements("ingredient"))
                db.ingredients.Add(new IngredientData { upgradeCost = Csv(Attr(d, "upgradeCost")), priceIncrease = I(Attr(d, "priceIncrease")) });
            foreach (var d in root.Elements("hardware"))
            {
                db.hardware.Add(new HardwareData
                {
                    name = Attr(d, "name"),
                    upgradeCost = Csv(Attr(d, "upgradeCost")),
                    defaultPrice = I(Attr(d, "defaultPrice")),
                    priceIncrease = I(Attr(d, "priceIncrease")),
                    startingTarget = I(Attr(d, "startingTarget")),
                    reduction = I(Attr(d, "Reduction")),
                });
            }
            return Save(db, "DishDatabase");
        }

        static CustomerDatabase ImportCustomers()
        {
            var db = ScriptableObject.CreateInstance<CustomerDatabase>();
            foreach (var c in Load("Customer.xml").Elements("customer"))
            {
                db.customers.Add(new CustomerData
                {
                    name = Attr(c, "name"),
                    textureName = Attr(c, "textureName"),
                    patience = F(Attr(c, "patience")),
                    tipsBonus = F(Attr(c, "tipsBonus")),
                });
            }
            return Save(db, "CustomerDatabase");
        }

        static WindowElement Element(XElement w, string name)
        {
            var e = w.Element(name);
            if (e == null) return new WindowElement();
            return new WindowElement
            {
                visible = Attr(e, "visible") == "true",
                offsetX = F(Attr(e, "offsetX")),
                offsetY = F(Attr(e, "offsetY")),
                text = e.Value.Trim(),
            };
        }

        static LevelEventDatabase ImportLevelEvents()
        {
            var db = ScriptableObject.CreateInstance<LevelEventDatabase>();
            foreach (var d in Load("LevelEvent.xml").Elements("day"))
            {
                var day = new DayEventsData { alwaysShow = Attr(d, "alwaysShow") == "true" };
                foreach (var w in d.Elements("window"))
                {
                    day.windows.Add(new EventWindowData
                    {
                        size = Attr(w, "size", "big"),
                        alwaysShow = Attr(w, "alwaysShow") == "true",
                        title = Element(w, "title"),
                        info = Element(w, "info"),
                        leftButton = Element(w, "leftButton"),
                        rightButton = Element(w, "rightButton"),
                        uniButton = Element(w, "uniButton"),
                        texture = Child(w, "texture"),
                    });
                }
                db.days.Add(day);
            }
            return Save(db, "LevelEventDatabase");
        }

        static DialogueDatabase ImportDialogues()
        {
            var db = ScriptableObject.CreateInstance<DialogueDatabase>();
            foreach (var d in Load("Tutorial_and_Dialogue.xml").Elements("dialogue"))
            {
                var dialogue = new DialogueData();
                foreach (var t in d.Elements("text"))
                {
                    dialogue.lines.Add(new DialogueLine
                    {
                        proceedType = Attr(t, "proceedType", "button"),
                        buttonText = Attr(t, "buttonText"),
                        posX = F(Attr(t, "posX")),
                        posY = F(Attr(t, "posY")),
                        arrowX = Attr(t, "arrowX"),
                        arrowY = Attr(t, "arrowY"),
                        arrowDirection = I(Attr(t, "arrowDirection"), 0), // int("") = 0 in the original
                        blockNext = Attr(t, "blockNext") == "true",
                        text = t.Value.Trim(),
                    });
                }
                db.dialogues.Add(dialogue);
            }
            return Save(db, "DialogueDatabase");
        }

        static DessertDatabase ImportDesserts()
        {
            var root = Load("MakeDessert.xml");
            var db = ScriptableObject.CreateInstance<DessertDatabase>();
            foreach (var i in root.Elements("item"))
            {
                db.items.Add(new DessertItemData
                {
                    name = Attr(i, "name"),
                    type = I(Attr(i, "type")),
                    posX = F(Attr(i, "posX")),
                    posY = F(Attr(i, "posY")),
                    worth = I(Attr(i, "worth")),
                });
            }
            foreach (var s in root.Elements("sets"))
                db.sets.Add(new DessertSetData { level = I(Attr(s, "level")), types = Csv(Attr(s, "type")) });
            return Save(db, "DessertDatabase");
        }

        static List<HiddenObjectLayout> ImportHiddenObjects()
        {
            var list = new List<HiddenObjectLayout>();
            foreach (var file in Directory.GetFiles(RoryPaths.DataSource, "HO*_data.xml").OrderBy(f => f, StringComparer.Ordinal))
            {
                var id = Path.GetFileName(file).Replace("_data.xml", "");
                var layout = ScriptableObject.CreateInstance<HiddenObjectLayout>();
                layout.sceneId = id;
                // The HO files declare a default namespace, so match on local names.
                foreach (var i in Parse(file).Elements().Where(e => e.Name.LocalName == "item"))
                    layout.items.Add(new HiddenObjectPlacement { name = Attr(i, "name"), locX = F(Attr(i, "locX")), locY = F(Attr(i, "locY")) });
                RoryAssets.EnsureFolder(RoryPaths.HiddenObjectData);
                list.Add(Save(layout, "HiddenObjects/" + id));
            }
            return list;
        }
    }
}
