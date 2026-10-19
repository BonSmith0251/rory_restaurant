using UnityEngine;

namespace Rory
{
    /// <summary>
    /// Full screen / windowed switching. The game renders at 1920x1080 in both modes: full screen (borderless,
    /// scaled to the monitor if it is not 1080p) or a 1920x1080 window.
    /// </summary>
    public static class DisplayMode
    {
        public const int Width = 1920, Height = 1080;

        public static void SetFullScreen(bool full)
        {
            if (Application.isEditor) return; // the Game view decides the size in the editor
            Screen.SetResolution(Width, Height, full ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }
    }
}
