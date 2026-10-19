using System;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The juicer or the fryer: machine button, the "filling" layer, the ready product, the copy of the product that is
    /// dragged to a customer (tmpDrink / tmpFries) and the percentage text. The machine fills by one per collected tile
    /// while its layer is shown; at the target the product is ready ("Drag to Customer"). Logic in
    /// <see cref="Match3Controller"/> (moveDrink / moveFries, remakeJuice / remakeFries).
    /// </summary>
    public class HardwareStation : MonoBehaviour
    {
        public enum Phase { Touch, Moved, Ended }

        [SerializeField] bool isJuicer;
        [SerializeField] Button machine;
        [SerializeField] Image layer;
        [SerializeField] Image product;
        [SerializeField] Image dragCopy;
        [SerializeField] TextMeshProUGUI percentage;
        [Tooltip("Home position of the dragged copy (stage pixels, centre)")]
        [SerializeField] Vector2 home;

        public bool IsJuicer => isJuicer;
        public Button Machine => machine;
        public Image Layer => layer;
        public Image Product => product;
        public Image DragCopy => dragCopy;
        public TextMeshProUGUI Percentage => percentage;
        public Vector2 Home => home;

        /// <summary>removedTileCountJuice / removedTileCountFries.</summary>
        public int removed;
        /// <summary>juiceTarget / friesTarget.</summary>
        public int target;

        /// <summary>Touch on the machine or on the dragged copy, with the stage position.</summary>
        public event Action<HardwareStation, Phase, Vector2> Touched;

        public bool LayerVisible => layer.gameObject.activeSelf;

        void Awake()
        {
            Hook(machine.gameObject);
            Hook(dragCopy.gameObject);
        }

        void Hook(GameObject go)
        {
            if (!go.TryGetComponent(out HardwareTouch f)) f = go.AddComponent<HardwareTouch>();
            f.station = this;
        }

        internal void Report(Phase phase, PointerEventData e) =>
            Touched?.Invoke(this, phase, UIStage.Current != null ? UIStage.Current.ScreenToStage(e.position) : e.position);

        /// <summary>Hidden entirely (hardware level 0).</summary>
        public void HideAll()
        {
            layer.gameObject.SetActive(false);
            percentage.gameObject.SetActive(false);
            machine.gameObject.SetActive(false);
            product.gameObject.SetActive(false);
        }

        public void ShowPercentage() => percentage.text = (int)((double)removed / target * 100) + "%";
    }
}
