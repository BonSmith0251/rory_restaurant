using System;
using System.Collections.Generic;
using Rory.Tweening;
using Rory.UI;
using UnityEngine;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// tweenTileAr: 25 hidden piece images that fly from the board to a customer (an ingredient to its order, a cookie
    /// to the least patient customer). Drawn above the board effects. A flight needs a free image; with all 25 in the air
    /// nothing flies (and the original then did nothing either).
    /// </summary>
    public class FlyingTiles : MonoBehaviour
    {
        public const int Count = 25;

        [SerializeField] TileView prefab;

        readonly List<TileView> views = new List<TileView>();
        readonly List<bool> busy = new List<bool>();

        void Awake()
        {
            for (int i = 0; i < Count; i++)
            {
                var v = Instantiate(prefab, transform, false);
                v.name = $"FlyingTile{i}";
                ((RectTransform)v.transform).SetStagePosition(-150, -150);
                v.gameObject.SetActive(false);
                views.Add(v);
                busy.Add(false);
            }
        }

        public int InFlight
        {
            get
            {
                int n = 0;
                foreach (var b in busy) if (b) n++;
                return n;
            }
        }

        /// <summary>First free image index, or -1.</summary>
        public int FirstFree()
        {
            for (int i = 0; i < Count; i++) if (!busy[i]) return i;
            return -1;
        }

        /// <summary>
        /// Shows image <paramref name="index"/> at (x, y) with <paramref name="sprite"/> and tweens it linearly to
        /// (toX, toY); <paramref name="onArrived"/> runs after it was hidden again.
        /// </summary>
        public void Fly(int index, Sprite sprite, float x, float y, float toX, float toY, float duration, Action onArrived)
        {
            var v = views[index];
            busy[index] = true;
            v.gameObject.SetActive(true);
            v.SetSprite(sprite);
            var rt = (RectTransform)v.transform;
            rt.localScale = Vector3.one;
            rt.SetStagePosition(x, y);
            rt.KillTweens();
            rt.TweenAnchoredPosition(StageLayout.ToAnchored(toX, toY), duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                v.gameObject.SetActive(false);
                rt.SetStagePosition(-150, -150);
                busy[index] = false;
                onArrived?.Invoke();
            });
        }
    }
}
