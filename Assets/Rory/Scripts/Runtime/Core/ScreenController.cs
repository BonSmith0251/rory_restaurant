using Rory.Audio;
using Rory.Data;
using Rory.Save;
using UnityEngine;

namespace Rory
{
    /// <summary>
    /// Base class of the per-scene controllers (MainMenuController, KitchenController, ...).
    /// The scene fades in once <see cref="IsReady"/> is true.
    /// </summary>
    public abstract class ScreenController : MonoBehaviour
    {
        public virtual bool IsReady => true;

        protected static GameManager Game => GameManager.Instance;
        protected static GameSession Session => GameManager.Instance.Session;
        protected static PlayerProfile Player => GameManager.Player;
        protected static GameDatabase Db => GameDatabase.Instance;
        protected static AudioManager Sound => GameManager.Instance.Audio;

        /// <summary>Called after the fade-in finished (start intro animations or tutorials here).</summary>
        public virtual void OnScreenVisible() { }

        protected static void GoTo(GameScene scene) => GameManager.Instance.Flow.Go(scene);
    }
}
