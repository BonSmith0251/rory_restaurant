using System.Collections.Generic;
using Rory.Screens;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/Splash.unity (the first scene of the build): the branding splashes stretched to the stage.</summary>
    public static class SplashBuilder
    {
        static readonly string[] Splashes = { "splash1", "splash2" };

        [MenuItem("Rory/Build/Scenes/Splash")]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var images = new List<Object>();
            foreach (var name in Splashes)
            {
                var sprite = RoryAssets.Load<Sprite>($"{RoryPaths.Branding}/{name}.jpg");
                var img = UIBuild.Image(stage.transform, name, sprite);
                UIBuild.Stretch(img.rectTransform);
                img.color = new Color(1, 1, 1, 0);
                images.Add(img);
            }
            // Full-screen transparent click catcher on top (a click skips the current splash).
            var catcher = UIBuild.Quad(stage.transform, "ClickToSkip", 0, 0, 1024, 600, new Color(0, 0, 0, 0), true);
            var controller = catcher.gameObject.AddComponent<SplashController>();
            UIBuild.SetArray(controller, "splashes", images);
            UIBuild.SaveScene("Splash", first: true);
        }
    }
}
