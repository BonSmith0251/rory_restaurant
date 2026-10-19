using Rory.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// Display of one <see cref="BoardTile"/> (the Match3Object image): centre pivot, native size of its ingredient
    /// sprite, position / scale from the model, and the hover glow (the original's yellow BlurFilter glow, drawn here as
    /// an outline). Prefab: Prefabs/Match3/Tile.prefab.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TileView : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] Outline glow;

        public BoardTile Tile { get; private set; }
        int shownType = -1;
        RectTransform rt;

        public Image Image => image;

        public void Bind(BoardTile tile)
        {
            Tile = tile;
            shownType = -1;
            rt = (RectTransform)transform;
            Sync();
        }

        public static Sprite SpriteFor(int type) => GameDatabase.Instance.sprites.Get("game", Match3Board.TextureName(type));

        /// <summary>Copies the model's state (called every frame by <see cref="BoardView"/>).</summary>
        public void Sync()
        {
            if (Tile == null) return;
            if (shownType != Tile.type)
            {
                shownType = Tile.type;
                SetSprite(SpriteFor(Tile.type));
            }
            rt.anchoredPosition = new Vector2(Tile.x, -Tile.y);
            rt.localScale = new Vector3(Tile.scale, Tile.scale, 1);
            if (glow != null && glow.enabled != Tile.glow) glow.enabled = Tile.glow;
        }

        /// <summary>texture + readjustSize + pivot = size / 2.</summary>
        public void SetSprite(Sprite sprite)
        {
            if (rt == null) rt = (RectTransform)transform;
            image.sprite = sprite;
            if (sprite != null) rt.sizeDelta = sprite.rect.size;
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
