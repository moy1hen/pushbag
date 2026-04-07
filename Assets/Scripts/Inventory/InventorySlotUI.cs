using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInventory
{
    public class InventorySlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject iconRoot;
        [SerializeField] private InventoryDraggableItem draggableItem;

        private InventoryUIManager manager;
        private int slotIndex;

        public void Initialize(InventoryUIManager owner, int index)
        {
            manager = owner;
            slotIndex = index;
            draggableItem.Bind(this);
        }

        public void EditorAssignReferences(Image assignedIconImage, GameObject assignedIconRoot, InventoryDraggableItem assignedDraggableItem)
        {
            iconImage = assignedIconImage;
            iconRoot = assignedIconRoot;
            draggableItem = assignedDraggableItem;
        }

        public void Refresh(InventoryItemData itemData)
        {
            bool hasItem = itemData != null;
            iconRoot.SetActive(hasItem);

            if (!hasItem)
            {
                iconImage.sprite = null;
                draggableItem.SetInteractable(false);
                return;
            }

            iconImage.sprite = itemData.icon;
            draggableItem.SetInteractable(true);
        }

        public int GetSlotIndex()
        {
            return slotIndex;
        }

        public InventoryUIManager GetManager()
        {
            return manager;
        }

        public void OnDrop(PointerEventData eventData)
        {
            InventoryDraggableItem draggedItem =
                eventData.pointerDrag != null
                    ? eventData.pointerDrag.GetComponent<InventoryDraggableItem>()
                    : null;

            if (draggedItem == null)
            {
                return;
            }

            InventorySlotUI fromSlot = draggedItem.GetOwnerSlot();
            if (fromSlot == null)
            {
                return;
            }

            manager.TrySwap(fromSlot.GetSlotIndex(), slotIndex);
        }
    }
}
