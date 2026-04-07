using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInventory
{
    public class InventoryDraggableItem : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private CanvasGroup canvasGroup;

        private InventorySlotUI ownerSlot;
        private RectTransform rectTransform;
        private Transform originalParent;
        private Vector2 originalAnchoredPosition;
        private bool canDrag;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            ResolveReferences();
        }

        public void Bind(InventorySlotUI slot)
        {
            ownerSlot = slot;
        }

        public void EditorAssignReferences(Canvas assignedRootCanvas, CanvasGroup assignedCanvasGroup)
        {
            rootCanvas = assignedRootCanvas;
            canvasGroup = assignedCanvasGroup;
        }

        public void SetInteractable(bool value)
        {
            ResolveReferences();
            canDrag = value;

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = value;
            }
        }

        public InventorySlotUI GetOwnerSlot()
        {
            return ownerSlot;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ResolveReferences();

            if (!canDrag || rootCanvas == null || canvasGroup == null)
            {
                return;
            }

            originalParent = transform.parent;
            originalAnchoredPosition = rectTransform.anchoredPosition;

            transform.SetParent(rootCanvas.transform, true);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.75f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag || rootCanvas == null)
            {
                return;
            }

            rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag || canvasGroup == null)
            {
                return;
            }

            transform.SetParent(originalParent, true);
            rectTransform.anchoredPosition = originalAnchoredPosition;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        private void ResolveReferences()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (rootCanvas == null)
            {
                rootCanvas = GetComponentInParent<Canvas>();
            }
        }
    }
}
