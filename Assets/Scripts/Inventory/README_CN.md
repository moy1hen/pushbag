# SimpleInventory 使用说明

> Unity 里请使用 `C#`，不是 `C++`。

## 1. 场景准备

- 创建一个 `Canvas`
- 确保场景里有 `EventSystem`
- 在 `Canvas` 下创建一个空物体 `InventoryRoot`
- 给 `InventoryRoot` 挂一个 `Grid Layout Group`

## 2. 创建格子预制体

- 新建一个 UI `Image`，命名为 `Slot`
- `Slot` 根节点挂上 `InventorySlotUI`
- 在 `Slot` 下面再放一个子物体 `Item`
- `Item` 上挂：
  - `Image`
  - `CanvasGroup`
  - `InventoryDraggableItem`

## 3. Inspector 绑定

### `InventorySlotUI`

- `Icon Image` 绑定到 `Item` 上的 `Image`
- `Icon Root` 绑定到 `Item`
- `Draggable Item` 绑定到 `Item` 上的 `InventoryDraggableItem`

### `InventoryDraggableItem`

- `Root Canvas` 绑定到场景中的 `Canvas`
- `Canvas Group` 绑定到当前物体上的 `CanvasGroup`

## 4. 挂载管理器

- 在场景里新建空物体 `InventoryManager`
- 挂上 `InventoryUIManager`
- `Slot Prefab` 指向刚才做好的 `Slot` 预制体
- `Slot Root` 指向 `InventoryRoot`
- `Demo Icons` 随便拖几个测试图片进去

## 5. 运行效果

- 有图标的格子可以拖拽
- 拖到另一个格子上会交换位置
- 拖到无效区域会回到原位

## 6. 后续扩展

- 如果你想更像《背包乱斗》，下一步可以继续加：
  - 物品大小不一（1x1、1x2、2x2）
  - 物品旋转
  - 邻接加成
  - 自动合成
