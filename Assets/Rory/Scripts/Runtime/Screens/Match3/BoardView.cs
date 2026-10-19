using System.Collections.Generic;
using Rory.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// Draws a <see cref="Match3Board"/>: the 64 tile images (creation order = draw order, like the original
    /// addChild order), the 20 ice overlays and the three pools of 20 board effects (fire "api_", smoke "asap_",
    /// stars "star_", MovieClips at 18 fps that hide after one run). Layers are built by Match3Builder in the original
    /// order: tiles, touch quad, ice, effects.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        public const int EffectsPerKind = 20;
        public const float EffectFps = 18;

        [Header("Layers")]
        [SerializeField] RectTransform tileLayer;
        [SerializeField] RectTransform frozenLayer;
        [SerializeField] RectTransform effectLayer;

        [Header("Prefabs")]
        [SerializeField] TileView tilePrefab;
        [SerializeField] Image frozenPrefab;
        [SerializeField] SpriteAnimator firePrefab;
        [SerializeField] SpriteAnimator smokePrefab;
        [SerializeField] SpriteAnimator starPrefab;

        readonly List<TileView> tiles = new List<TileView>();
        readonly List<Image> frozen = new List<Image>();
        readonly List<SpriteAnimator> fire = new List<SpriteAnimator>(), smoke = new List<SpriteAnimator>(), star = new List<SpriteAnimator>();

        public Match3Board Board { get; private set; }
        public IReadOnlyList<TileView> Tiles => tiles;

        public void Bind(Match3Board board)
        {
            Board = board;
            foreach (var t in board.AllTiles)
            {
                var v = Instantiate(tilePrefab, tileLayer, false);
                v.name = $"Tile{t.id}";
                v.Bind(t);
                tiles.Add(v);
            }
            for (int i = 0; i < Match3Board.FrozenOverlayCount; i++)
            {
                var f = Instantiate(frozenPrefab, frozenLayer, false);
                f.name = $"FrozenTile{i}";
                f.gameObject.SetActive(false);
                frozen.Add(f);
            }
            // creation order of the original: fire, smoke, star, fire, smoke, star, ...
            for (int i = 0; i < EffectsPerKind; i++)
            {
                fire.Add(NewEffect(firePrefab, $"Fire{i}"));
                smoke.Add(NewEffect(smokePrefab, $"Smoke{i}"));
                star.Add(NewEffect(starPrefab, $"Star{i}"));
            }
            Sync();
        }

        SpriteAnimator NewEffect(SpriteAnimator prefab, string name)
        {
            var e = Instantiate(prefab, effectLayer, false);
            e.name = name;
            e.playOnEnable = false;
            e.loop = false;
            e.fps = EffectFps;
            e.Completed += () => e.gameObject.SetActive(false);
            e.gameObject.SetActive(false);
            return e;
        }

        /// <summary>playBoardEffect: the first free clip of the kind plays at (x, y); nothing when all 20 are busy.</summary>
        public void PlayEffect(BoardEffectKind kind, float x, float y)
        {
            var pool = kind == BoardEffectKind.Fire ? fire : kind == BoardEffectKind.Smoke ? smoke : star;
            foreach (var e in pool)
            {
                if (e.gameObject.activeSelf) continue;
                e.gameObject.SetActive(true);
                ((RectTransform)e.transform).SetStagePosition(x, y);
                e.Play();
                return;
            }
        }

        public int ActiveEffects(BoardEffectKind kind)
        {
            var pool = kind == BoardEffectKind.Fire ? fire : kind == BoardEffectKind.Smoke ? smoke : star;
            int n = 0;
            foreach (var e in pool) if (e.gameObject.activeSelf) n++;
            return n;
        }

        void LateUpdate() => Sync();

        public void Sync()
        {
            if (Board == null) return;
            foreach (var t in tiles) t.Sync();
            for (int i = 0; i < frozen.Count; i++)
            {
                var m = Board.Frozen[i];
                var img = frozen[i];
                if (img.gameObject.activeSelf != m.visible) img.gameObject.SetActive(m.visible);
                img.rectTransform.anchoredPosition = new Vector2(m.x, -m.y);
            }
        }

        public TileView ViewOf(BoardTile tile) => tiles[tile.id];
    }
}
