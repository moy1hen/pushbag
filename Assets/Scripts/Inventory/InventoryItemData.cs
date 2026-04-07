using UnityEngine;

namespace SimpleInventory
{
    [System.Serializable]
    public class InventoryItemData
    {
        public string itemId;
        public Sprite icon;

        public InventoryItemData(string itemId, Sprite icon)
        {
            this.itemId = itemId;
            this.icon = icon;
        }
    }
}
