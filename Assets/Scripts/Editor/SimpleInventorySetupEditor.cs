using System.IO;
using SimpleInventory;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class SimpleInventorySetupEditor
{
    private const string GeneratedFolder = "Assets/GeneratedInventory";
    private const string PrefabFolder = GeneratedFolder + "/Prefabs";
    private const string IconFolder = GeneratedFolder + "/Icons";
    private const string SlotPrefabPath = PrefabFolder + "/InventorySlot.prefab";

    [MenuItem("Tools/Simple Inventory/Create Demo Setup")]
    public static void CreateDemoSetup()
    {
        EnsureFolders();

        Canvas canvas = EnsureCanvas();
        EnsureEventSystem();
        Transform inventoryRoot = EnsureInventoryRoot(canvas.transform);
        InventorySlotUI slotPrefab = CreateOrUpdateSlotPrefab(canvas);
        Sprite[] demoIcons = CreateDemoIcons();
        InventoryUIManager manager = EnsureInventoryManager(slotPrefab, inventoryRoot, demoIcons);

        Selection.activeGameObject = manager.gameObject;
        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        AssetDatabase.SaveAssets();

        Debug.Log("Simple Inventory demo setup created. Press Play to test drag-and-swap.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(GeneratedFolder))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedInventory");
        }

        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder(GeneratedFolder, "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder(IconFolder))
        {
            AssetDatabase.CreateFolder(GeneratedFolder, "Icons");
        }
    }

    private static Canvas EnsureCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            return canvas;
        }

        GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private static Transform EnsureInventoryRoot(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("InventoryRoot");
        if (existing != null)
        {
            return existing;
        }

        GameObject root = new GameObject("InventoryRoot", typeof(RectTransform), typeof(GridLayoutGroup));
        root.transform.SetParent(canvasTransform, false);

        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(520f, 420f);
        rect.anchoredPosition = Vector2.zero;

        GridLayoutGroup grid = root.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(88f, 88f);
        grid.spacing = new Vector2(8f, 8f);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        grid.childAlignment = TextAnchor.MiddleCenter;

        return root.transform;
    }

    private static InventorySlotUI CreateOrUpdateSlotPrefab(Canvas canvas)
    {
        GameObject slotRoot = new GameObject("InventorySlot", typeof(RectTransform), typeof(Image), typeof(InventorySlotUI));
        RectTransform slotRect = slotRoot.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(88f, 88f);

        Image slotBackground = slotRoot.GetComponent<Image>();
        slotBackground.color = new Color(0.17f, 0.18f, 0.22f, 0.95f);
        slotBackground.raycastTarget = true;

        GameObject itemObject = new GameObject("Item", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(InventoryDraggableItem));
        itemObject.transform.SetParent(slotRoot.transform, false);

        RectTransform itemRect = itemObject.GetComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = new Vector2(64f, 64f);
        itemRect.anchoredPosition = Vector2.zero;

        Image itemImage = itemObject.GetComponent<Image>();
        itemImage.preserveAspect = true;
        itemImage.raycastTarget = true;

        CanvasGroup canvasGroup = itemObject.GetComponent<CanvasGroup>();
        InventoryDraggableItem draggableItem = itemObject.GetComponent<InventoryDraggableItem>();
        draggableItem.EditorAssignReferences(canvas, canvasGroup);

        InventorySlotUI slotUI = slotRoot.GetComponent<InventorySlotUI>();
        slotUI.EditorAssignReferences(itemImage, itemObject, draggableItem);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slotRoot, SlotPrefabPath);
        Object.DestroyImmediate(slotRoot);

        return prefab.GetComponent<InventorySlotUI>();
    }

    private static Sprite[] CreateDemoIcons()
    {
        Color[] colors =
        {
            new Color(0.87f, 0.32f, 0.24f),
            new Color(0.22f, 0.63f, 0.33f),
            new Color(0.23f, 0.48f, 0.87f),
            new Color(0.88f, 0.70f, 0.22f),
            new Color(0.64f, 0.34f, 0.81f),
            new Color(0.18f, 0.74f, 0.76f)
        };

        Sprite[] sprites = new Sprite[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            string iconPath = $"{IconFolder}/DemoIcon_{i + 1}.png";

            if (!File.Exists(iconPath))
            {
                Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[64 * 64];
                for (int p = 0; p < pixels.Length; p++)
                {
                    pixels[p] = colors[i];
                }

                texture.SetPixels(pixels);
                texture.Apply();
                File.WriteAllBytes(iconPath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
            }
        }

        AssetDatabase.Refresh();

        for (int i = 0; i < colors.Length; i++)
        {
            string iconPath = $"{IconFolder}/DemoIcon_{i + 1}.png";
            TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        }

        return sprites;
    }

    private static InventoryUIManager EnsureInventoryManager(InventorySlotUI slotPrefab, Transform slotRoot, Sprite[] demoIcons)
    {
        InventoryUIManager manager = Object.FindFirstObjectByType<InventoryUIManager>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject("InventoryManager", typeof(InventoryUIManager));
            manager = managerObject.GetComponent<InventoryUIManager>();
        }

        manager.EditorAssignSetup(slotPrefab, slotRoot, demoIcons);
        return manager;
    }
}
