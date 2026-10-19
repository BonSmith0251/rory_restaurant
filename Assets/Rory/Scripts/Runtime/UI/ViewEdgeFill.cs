using UnityEngine;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>
    /// A mirrored edge strip that extends a 1024-wide background into the 16:9 side margins (built by
    /// UIBuild.Image). It follows the background's colour/alpha and visibility, so fades and hides still work.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class ViewEdgeFill : MonoBehaviour
    {
        [SerializeField] Graphic source;
        RawImage strip;

        public void Init(Graphic background)
        {
            source = background;
            Sync();
        }

        void Awake() => strip = GetComponent<RawImage>();

        void LateUpdate() => Sync();

        void Sync()
        {
            if (strip == null) strip = GetComponent<RawImage>();
            if (source == null) return;
            if (strip.color != source.color) strip.color = source.color;
            if (strip.enabled != source.enabled) strip.enabled = source.enabled;
        }
    }
}
