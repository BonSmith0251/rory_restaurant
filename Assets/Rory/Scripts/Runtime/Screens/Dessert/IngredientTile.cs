using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Dessert
{
    /// <summary>
    /// One of the 14 invisible 100x100 touch areas over the ingredient stations (the alpha-0 Quads named "0".."13"
    /// of MiniGameMakeDessert). A press (TouchPhase.BEGAN) picks ingredient <see cref="id"/>, the index into
    /// <c>DessertDatabase.items</c>.
    /// </summary>
    public class IngredientTile : MonoBehaviour, IPointerDownHandler
    {
        [Tooltip("Ingredient index (DessertDatabase.items)")]
        public int id;
        [SerializeField] DessertController controller;

        public void OnPointerDown(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left || controller == null) return;
            controller.TouchIngredient(id);
        }
    }
}
