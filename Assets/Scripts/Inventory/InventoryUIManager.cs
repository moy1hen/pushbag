using System.Collections.Generic;
using UnityEngine;

namespace SimpleInventory
{
    public class InventoryUIManager : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private int slotCount = 20;
        [SerializeField] private InventorySlotUI slotPrefab;
        [SerializeField] private Transform slotRoot;

        [Header("Demo Items")]
        [SerializeField] private Sprite[] demoIcons;

        private readonly List<InventorySlotUI> slots = new();
        private InventoryItemData[] items;

        private void Start()
        {
            BuildSlots();
            CreateDemoData();
            RefreshAllSlots();
        }

        private void BuildSlots()
        {
            slots.Clear();
            items = new InventoryItemData[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlotUI slot = Instantiate(slotPrefab, slotRoot);
                slot.Initialize(this, i);
                slots.Add(slot);
            }
        }

        private void CreateDemoData()
        {
            int demoCount = Mathf.Min(demoIcons.Length, items.Length);

            for (int i = 0; i < demoCount; i++)
            {
                items[i] = new InventoryItemData($"Item_{i + 1}", demoIcons[i]);
            }
        }

        public void EditorAssignSetup(InventorySlotUI assignedSlotPrefab, Transform assignedSlotRoot, Sprite[] assignedDemoIcons)
        {
            slotPrefab = assignedSlotPrefab;
            slotRoot = assignedSlotRoot;
            demoIcons = assignedDemoIcons;
        }

        public InventoryItemData GetItem(int slotIndex)
        {
            if (!IsValidIndex(slotIndex))
            {
                return null;
            }

            return items[slotIndex];
        }

        public void TrySwap(int fromIndex, int toIndex)
        {
            if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex) || fromIndex == toIndex)
            {
                RefreshAllSlots();
                return;
            }

            (items[fromIndex], items[toIndex]) = (items[toIndex], items[fromIndex]);
            RefreshAllSlots();
        }

        public void RefreshAllSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Refresh(GetItem(i));
            }
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < items.Length;
        }
    }
}
