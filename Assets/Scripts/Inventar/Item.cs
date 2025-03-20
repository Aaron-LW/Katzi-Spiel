using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item
{
    public int ID;
    public string Name;
    public int MaxStackSize;
    public Sprite ItemSprite;

    public Item(int id, string name, int maxstacksize, Sprite itemsprite)
    {
        ID = id;
        Name = name;
        MaxStackSize = maxstacksize;
        ItemSprite = itemsprite;
    }
}

public class ItemStack
{
    public Item Item;
    public int Amount;
    public int SlotIndex;

    public ItemStack(Item item, int amount, int slotindex = -1)
    {
        Item = item;
        Amount = amount;
        SlotIndex = slotindex;
    }
}

public class Inventar
{
    public int ID;
    public int Slots;
    public GameObject InventarUIGameObject;
    public List<ItemStack> Items;
    public int SecondaryInventar = -1;

    public Inventar(int id, int slots, GameObject inventaruigameobject, List<ItemStack> items, int secondaryinventar = -1)
    {
        ID = id;
        Slots = slots;
        InventarUIGameObject = inventaruigameobject;
        Items = items;
        SecondaryInventar = secondaryinventar;
    }
}