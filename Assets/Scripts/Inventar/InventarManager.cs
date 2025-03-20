using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventarManager : MonoBehaviour
{
    public static InventarManager Instance { get; private set; }

    [HideInInspector] public List<Inventar> Inventare = new List<Inventar>();
    [HideInInspector] public List<Item> InitialisedItems = new List<Item>();

    public GameObject InventarGameObject;
    public GameObject HotbarGameObject;

    [HideInInspector] public int CurrHotbarSlot = 0;
    [HideInInspector] public bool InventarOpen = false;

    [HideInInspector] public ItemStack CurrDraggingItemStack;

    public CameraMovement CameraMovement;
    public GameObject CurrDragging;

    [Header("ItemSprites")]
    public Sprite HolzSprite;
    public Sprite SteinSprite;
    public Sprite FaserSprite;
    public Sprite KohleSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        /*  
            0: Blank
            1: Holz
            2: Stein
            3: Fasern
            4: Kohle
        */

        InitialisedItems.Add(new Item(0, "Blank", 0, null));
        InitialisedItems.Add(new Item(1, "Holz", 999, HolzSprite));
        InitialisedItems.Add(new Item(2, "Stein", 999, SteinSprite));
        InitialisedItems.Add(new Item(3, "Fasern", 999, FaserSprite));
        InitialisedItems.Add(new Item(4, "Kohle", 999, KohleSprite));

        IntitialiseInventory(0, 21, InventarGameObject, 1);
        IntitialiseInventory(1, 7, HotbarGameObject, 0);

        CurrDraggingItemStack = new ItemStack(InitialisedItems[0], 0);
        UpdateDragging();

        InventarUI.Instance.UpdateInventory(Inventare[0]);
        InventarUI.Instance.UpdateInventory(Inventare[1]);

        QuestManager.Instance.InitManager();
        CraftingManager.Instance.InitManager();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (InventarOpen)
            {
                InventarGameObject.SetActive(false);
                CameraMovement.enabled = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                InventarOpen = false;

                InventarUI.Instance.UpdateInventory(Inventare[1]);
            }
            else
            {
                InventarGameObject.SetActive(true);
                CameraMovement.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                InventarOpen = true;

                InventarUI.Instance.UpdateInventory(Inventare[0]);
                InventarUI.Instance.UpdateInventory(Inventare[1]);
            }
        }

        if (Input.GetKey(KeyCode.F))
        {
            AddItem(1, Mathf.FloorToInt(Random.Range(1, 5)), 10);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            RemoveItem(0, 1, 1332, 1);
        }

        CurrDragging.gameObject.transform.position = Input.mousePosition;

        if (Input.mouseScrollDelta.y > 0 && CurrHotbarSlot > 0) { CurrHotbarSlot -= 1; InventarUI.Instance.UpdateInventory(Inventare[1]); }
        if (Input.mouseScrollDelta.y < 0 && CurrHotbarSlot < Inventare[1].Slots - 1) { CurrHotbarSlot += 1; InventarUI.Instance.UpdateInventory(Inventare[1]); }
    }

    public void AddItem(int InventarIndex, int ID, int Amount, int SlotIndex = -1, bool SecondaryInventar = false)
    {
        if (ID > InitialisedItems.Count - 1) { Debug.LogError("Es wird versucht ein Item hinzuzufügen dass nicht existiert"); return; }
        if (InventarIndex > Inventare.Count) { Debug.LogError("Es wird versucht in einem Inventar zu addieren dass nicht exisitert"); return; }

        List<int> FreeSlots = new List<int>();

        if (SlotIndex != -1)
        {
            Inventare[InventarIndex].Items[SlotIndex] = new ItemStack(InitialisedItems[ID], Amount, SlotIndex);
            InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);
            return;
        }

        for (int i = 0; i < Inventare[InventarIndex].Slots; i++)
        {
            ItemStack CurrItemStack = Inventare[InventarIndex].Items[i];
            Item CurrItem = CurrItemStack.Item;

            if (CurrItem.ID == 0)
            {
                FreeSlots.Add(i);
                continue;
            }

            if (CurrItem.ID == ID && CurrItemStack.Amount != CurrItem.MaxStackSize)
            {
                if (CurrItemStack.Amount + Amount > CurrItem.MaxStackSize)
                {
                    Amount -= CurrItem.MaxStackSize - CurrItemStack.Amount;
                    CurrItemStack.Amount = CurrItem.MaxStackSize;
                }
                else
                {
                    CurrItemStack.Amount += Amount;
                    InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

                    return;
                }
            }
        }

        for (int i = 0; i < FreeSlots.Count; i++)
        {
            if (Amount > InitialisedItems[ID].MaxStackSize)
            {
                Inventare[InventarIndex].Items[FreeSlots[i]] = new ItemStack(InitialisedItems[ID], InitialisedItems[ID].MaxStackSize, FreeSlots[i]);
                Amount -= InitialisedItems[ID].MaxStackSize;
            }
            else
            {
                Inventare[InventarIndex].Items[FreeSlots[i]] = new ItemStack(InitialisedItems[ID], Amount, FreeSlots[i]);
                InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

                return;
            }
        }

        if (Inventare[InventarIndex].SecondaryInventar != -1 && !SecondaryInventar)
        {
            AddItem(Inventare[InventarIndex].SecondaryInventar, ID, Amount, -1, SecondaryInventar = true);
            InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

            return;
        }

        Debug.LogError("Nicht Genug Platz im Inventar um Item hinzuzufügen");
    }

    public void RemoveItem(int InventarIndex, int ID, int Amount, int SlotIndex = -1, bool ReverseInventory = true, bool SecondaryInventar = false)
    {
        Inventar TempInventar = Inventare[InventarIndex];

        if (SlotIndex != -1)
        {
            if (Inventare[InventarIndex].Items[SlotIndex].Item.ID == ID)
            {
                if (Inventare[InventarIndex].Items[SlotIndex].Amount - Amount > 0)
                {
                    Inventare[InventarIndex].Items[SlotIndex].Amount -= Amount;
                    InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

                    return;
                }
                else
                {
                    Amount -= Inventare[InventarIndex].Items[SlotIndex].Amount;
                    TempInventar.Items[SlotIndex] = new ItemStack(InitialisedItems[0], 0, SlotIndex);
                }
            }
        }

        if (ReverseInventory)
        {
            TempInventar.Items.Reverse();
        }

        for (int i = 0; i < Inventare[InventarIndex].Slots; i++)
        {
            ItemStack CurrItemStack = TempInventar.Items[i];
            Item CurrItem = CurrItemStack.Item;

            if (CurrItem.ID == ID)
            {
                if (CurrItemStack.Amount - Amount > 0)
                {
                    CurrItemStack.Amount -= Amount;

                    if (ReverseInventory)
                    {
                        TempInventar.Items.Reverse();
                    }

                    Inventare[InventarIndex] = TempInventar;
                    InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

                    return;
                }
                else
                {
                    Amount -= CurrItemStack.Amount;
                    TempInventar.Items[i] = new ItemStack(InitialisedItems[0], 0, i);

                    InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);
                }
            }

            if (Amount == 0)
            {
                if (ReverseInventory)
                {
                    TempInventar.Items.Reverse();
                }

                Inventare[InventarIndex] = TempInventar;
                InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);

                return;
            }
        }

        if (Amount > 0)
        {
            if (Inventare[InventarIndex].SecondaryInventar != -1 && !SecondaryInventar)
            {
                RemoveItem(Inventare[InventarIndex].SecondaryInventar, ID, Amount, -1, ReverseInventory, true);
                return;
            }

            Debug.LogError("Nicht genug Items zum entfernen gefunden");
        }
    }

    public void SwitchItem(int InventarIndex, int SlotIndex, bool quick = false)
    {
        ItemStack tempStack = Inventare[InventarIndex].Items[SlotIndex];

        if (quick)
        {
            RemoveItem(InventarIndex, tempStack.Item.ID, tempStack.Amount, tempStack.SlotIndex);
            AddItem(Inventare[InventarIndex].SecondaryInventar, tempStack.Item.ID, tempStack.Amount);
            return;
        }

        if (tempStack.Item.ID == CurrDraggingItemStack.Item.ID)
        {
            if (tempStack.Amount + CurrDraggingItemStack.Amount < tempStack.Item.MaxStackSize)
            {
                tempStack.Amount += CurrDraggingItemStack.Amount;
                CurrDraggingItemStack = new ItemStack(InitialisedItems[0], 0);
            }
            else
            {
                int tempAmount = tempStack.Amount;
                tempStack.Amount = tempStack.Item.MaxStackSize;
                CurrDraggingItemStack.Amount -= tempStack.Item.MaxStackSize - tempAmount;
            }
        }
        else
        {
            Inventare[InventarIndex].Items[SlotIndex] = CurrDraggingItemStack;
            CurrDraggingItemStack = tempStack;
        }

        UpdateDragging();
        InventarUI.Instance.UpdateInventory(Inventare[InventarIndex]);
    }

    public bool FindItems(List<ItemStack> Items)
    {
        List<ItemStack> FoundItems = new List<ItemStack>();

        for (int i = 0; i < 2; i++)
        {
            Inventar CurrInventar = Inventare[i];

            for (int j = 0; j < CurrInventar.Slots; j++)
            {
                ItemStack CurrItemStack = CurrInventar.Items[j];
                Item CurrItem = CurrItemStack.Item;

                if (CurrItem.ID == 0) { continue; }

                for (int k = 0; k < Items.Count; k++)
                {
                    if (CurrItem.ID == Items[k].Item.ID)
                    {
                        for (int l = 0; l < FoundItems.Count - 1; l++)
                        {
                            if (FoundItems[l].Item.ID == CurrItem.ID)
                            {
                                FoundItems[l].Amount += CurrItemStack.Amount;
                                break;
                            }
                        }
                        FoundItems.Add(new ItemStack(CurrItem, CurrItemStack.Amount));
                    }
                }
            }
        }

        int FoundItemsCount = 0;

        for (int i = 0; i < FoundItems.Count; i++)
        {
            for (int j = 0; j < Items.Count; j++)
            {
                if (FoundItems[i].Item.ID == Items[j].Item.ID && FoundItems[i].Amount >= Items[j].Amount)
                {
                    FoundItemsCount++;
                }
            }
        }

        if (FoundItemsCount == Items.Count)
        {
            return true;
        }

        return false;
    }

    void IntitialiseInventory(int ID, int Slots, GameObject InventarUITransform, int SecondaryInventar = -1)
    {
        Inventare.Add(new Inventar(ID, Slots, InventarUITransform, new List<ItemStack>(), SecondaryInventar));

        for (int i = 0; i < Slots; i++)
        {
            Inventare[ID].Items.Add(new ItemStack(InitialisedItems[0], 0, i));
        }
    }

    void UpdateDragging()
    {
        Image img = CurrDragging.GetComponentInChildren<Image>();
        TMP_Text text = CurrDragging.GetComponentInChildren<TMP_Text>();

        if (InitialisedItems[CurrDraggingItemStack.Item.ID].ItemSprite != null)
        {
            img.color = Color.white;
            img.sprite = InitialisedItems[CurrDraggingItemStack.Item.ID].ItemSprite;
        }
        else
        {
            img.color = Color.clear;
        }

        if (CurrDraggingItemStack.Amount == 0)
        {
            text.text = "";
        }
        else
        {
            text.text = CurrDraggingItemStack.Amount.ToString();
        }
    }
}