// Inventory.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // 单例模式，方便全局访问

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    public int space = 20; // 背包容量
    public List<ItemStack> items = new List<ItemStack>(); // 存储物品堆叠的列表

    // 回调事件，当UI更新时通知
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // UI相关
    public GameObject inventoryPanel; // 背包UI面板
    public Transform slotsParent;     // 所有物品格子的父物体
    public GameObject slotPrefab;     // 物品格子预制体

    // 初始化UI格子
    void Start()
    {
        Debug.Log("Inventory Start方法被调用");
        
        // 如果没有设置UI引用，尝试自动创建或查找
        if (inventoryPanel == null)
        {
            Debug.LogWarning("inventoryPanel未设置，尝试自动创建UI");
            CreateInventoryUI();
        }
        
        // 检查必要的引用是否设置
        if (inventoryPanel == null)
        {
            Debug.LogError("inventoryPanel未设置，自动创建失败，请在Inspector中指定背包面板");
        }
        
        if (slotsParent == null)
        {
            Debug.LogError("slotsParent未设置，请在Inspector中指定物品格子的父物体");
        }
        
        if (slotPrefab == null)
        {
            Debug.LogError("slotPrefab未设置，请在Inspector中指定物品格子预制体");
        }
        
        // 根据容量创建足够多的格子
        for (int i = 0; i < space; i++)
        {
            Instantiate(slotPrefab, slotsParent);
        }
        
        Debug.Log("创建了 " + space + " 个物品格子");
        
        // 订阅UI更新事件
        onItemChangedCallback += UpdateUIOptimized;
        
        // 确保背包面板初始状态为隐藏
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("背包面板初始状态设置为隐藏");
        }
    }
    
    // 自动创建背包UI
    void CreateInventoryUI()
    {
        // 查找或创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("创建了新的Canvas");
        }
        
        // 创建背包面板
        inventoryPanel = new GameObject("InventoryPanel");
        inventoryPanel.transform.SetParent(canvas.transform, false);
        
        // 添加Image组件作为背景
        Image panelImage = inventoryPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // 半透明黑色背景
        
        // 设置RectTransform
        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.3f);
        panelRect.anchorMax = new Vector2(0.7f, 0.7f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // 创建物品格子容器
        GameObject slotsContainer = new GameObject("SlotsContainer");
        slotsContainer.transform.SetParent(inventoryPanel.transform, false);
        
        // 添加GridLayoutGroup组件
        GridLayoutGroup gridLayout = slotsContainer.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(60, 60);
        gridLayout.spacing = new Vector2(5, 5);
        gridLayout.padding = new RectOffset(10, 10, 10, 10);
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.childAlignment = TextAnchor.UpperLeft;
        
        // 设置RectTransform
        RectTransform containerRect = slotsContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = new Vector2(10, 10);
        containerRect.offsetMax = new Vector2(-10, -10);
        
        slotsParent = slotsContainer.transform;
        
        // 如果没有设置格子预制体，创建一个基本的
        if (slotPrefab == null)
        {
            CreateSlotPrefab();
        }
        
        Debug.Log("自动创建了背包UI结构");
    }
    
    // 创建物品格子预制体
    void CreateSlotPrefab()
    {
        // 创建格子预制体
        slotPrefab = new GameObject("Slot");
        
        // 添加Image组件作为背景
        Image slotImage = slotPrefab.AddComponent<Image>();
        slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 深灰色背景
        
        // 设置RectTransform
        RectTransform slotRect = slotPrefab.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(60, 60);
        
        // 创建物品图标Image
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(slotPrefab.transform, false);
        Image iconImage = iconObj.AddComponent<Image>();
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        iconImage.enabled = false; // 初始隐藏
        
        // 创建数量文本
        GameObject amountObj = new GameObject("AmountText");
        amountObj.transform.SetParent(slotPrefab.transform, false);
        Text amountText = amountObj.AddComponent<Text>();
        amountText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        amountText.fontSize = 12;
        amountText.color = Color.white;
        amountText.alignment = TextAnchor.LowerRight;
        RectTransform amountRect = amountObj.GetComponent<RectTransform>();
        amountRect.anchorMin = Vector2.zero;
        amountRect.anchorMax = Vector2.one;
        amountRect.offsetMin = new Vector2(0, 0);
        amountRect.offsetMax = new Vector2(0, 0);
        amountText.enabled = false; // 初始隐藏
        
        // 创建移除按钮
        GameObject removeObj = new GameObject("RemoveButton");
        removeObj.transform.SetParent(slotPrefab.transform, false);
        Button removeButton = removeObj.AddComponent<Button>();
        Image removeImage = removeObj.AddComponent<Image>();
        removeImage.color = Color.red;
        RectTransform removeRect = removeObj.GetComponent<RectTransform>();
        removeRect.sizeDelta = new Vector2(15, 15);
        removeRect.anchorMin = new Vector2(1, 1);
        removeRect.anchorMax = new Vector2(1, 1);
        removeRect.anchoredPosition = new Vector2(-5, -5);
        removeButton.interactable = false; // 初始不可交互
        
        // 添加InventorySlot组件并设置引用
        InventorySlot slot = slotPrefab.AddComponent<InventorySlot>();
        slot.icon = iconImage;
        slot.amountText = amountText;
        slot.removeButton = removeButton;
        
        Debug.Log("创建了物品格子预制体");
    }
    
    // 更新所有UI格子（优化版本）
    void UpdateUI()
    {
        Debug.Log($"更新背包UI，物品数量: {items.Count}");
        
        // 获取所有格子
        InventorySlot[] slots = slotsParent.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                // 如果有物品，检查是否需要更新
                ItemStack stack = items[i];
                if (slots[i].GetCurrentItem() != stack.item || slots[i].GetCurrentAmount() != stack.amount)
                {
                    slots[i].AddItem(stack);
                }
            }
            else
            {
                // 如果没有物品，检查是否需要清空
                if (slots[i].GetCurrentItem() != null)
                {
                    slots[i].ClearSlot();
                }
            }
        }
    }
    
    // 优化的UI更新方法，只更新变化的格子
    void UpdateUIOptimized()
    {
        // 获取所有格子
        InventorySlot[] slots = slotsParent.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                // 如果有物品，检查是否需要更新
                ItemStack stack = items[i];
                if (slots[i].GetCurrentItem() != stack.item || slots[i].GetCurrentAmount() != stack.amount)
                {
                    slots[i].AddItem(stack);
                }
            }
            else
            {
                // 如果没有物品，检查是否需要清空
                if (slots[i].GetCurrentItem() != null)
                {
                    slots[i].ClearSlot();
                }
            }
        }
    }


    void Update()
    {
        // 按I键打开/关闭背包
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I键被按下，切换背包显示状态");
            if (inventoryPanel != null)
            {
                bool newState = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(newState);
                Debug.Log("背包面板状态设置为: " + (newState ? "显示" : "隐藏"));
            }
            else
            {
                Debug.LogError("inventoryPanel为空，请确保在Inspector中设置了背包面板");
            }
        }
    }

    // 添加物品到背包
    public bool AddItem(ItemData itemToAdd, int amount = 1)
    {
        // 如果物品可堆叠，先尝试添加到已有的堆叠中
        if (itemToAdd.isStackable)
        {
            foreach (ItemStack stack in items)
            {
                if (stack.item == itemToAdd && !stack.IsFull())
                {
                    int remainder = stack.AddWithRemainder(amount);
                    if (remainder == 0)
                    {
                        // 通知UI更新
                        if (onItemChangedCallback != null)
                        {
                            onItemChangedCallback.Invoke();
                        }
                        return true;
                    }
                    else
                    {
                        amount = remainder;
                    }
                }
            }
        }
        
        // 如果还有剩余物品需要添加，检查背包空间
        while (amount > 0)
        {
            // 检查背包是否已满
            if (items.Count >= space)
            {
                Debug.Log("背包已满!");
                return false;
            }
            
            // 创建新的堆叠
            int stackAmount = Mathf.Min(amount, itemToAdd.maxStackSize);
            items.Add(new ItemStack(itemToAdd, stackAmount));
            amount -= stackAmount;
        }
        
        // 通知UI更新
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
            Debug.Log($"成功添加物品: {itemToAdd.itemName}, 数量: {amount}, 当前背包物品数量: {items.Count}");
        }
        
        return true;
    }

    // 从背包移除物品
    public void RemoveItem(ItemData itemToRemove, int amount = 1)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            ItemStack stack = items[i];
            if (stack.item == itemToRemove)
            {
                if (stack.amount <= amount)
                {
                    amount -= stack.amount;
                    items.RemoveAt(i);
                    
                    if (amount == 0) break;
                }
                else
                {
                    stack.Remove(amount);
                    break;
                }
            }
        }
        
        // 通知UI更新
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
    
    // 从背包移除整个堆叠
    public void RemoveStack(ItemStack stackToRemove)
    {
        items.Remove(stackToRemove);
        
        // 通知UI更新
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}

