using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventarUI : MonoBehaviour
{
    public static InventarUI Instance { get; private set; }

    public GameObject Slot;

    void Awake()
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

    public void UpdateInventory(Inventar Inventar)
    {
        foreach (Transform child in Inventar.InventarUIGameObject.transform)
        {
            if (child.CompareTag("ItemSlot"))
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < Inventar.Slots; i++)
        {
            ItemStack CurrItemStack = Inventar.Items[i];
            Item CurrItem = CurrItemStack.Item;

            GameObject InstantiatedSlot = Instantiate(Slot, Inventar.InventarUIGameObject.transform);

            ItemSlotScript CurrSlotScript = InstantiatedSlot.GetComponentInChildren<ItemSlotScript>();

            CurrSlotScript.CurrItemIndex = i;
            CurrSlotScript.CurrInventarIndex = Inventar.ID;

            if (Inventar.ID == 1 && InventarManager.Instance.CurrHotbarSlot == i && !InventarManager.Instance.InventarOpen)
            {
                InstantiatedSlot.GetComponent<Outline>().enabled = true;
            }

            Image[] Images = InstantiatedSlot.GetComponentsInChildren<Image>();

            if (CurrItem.ItemSprite == null)
            {
                Images[1].color = Color.clear;
            }
            else
            {
                Images[1].sprite = CurrItem.ItemSprite;
            }

            TMP_Text text = InstantiatedSlot.GetComponentInChildren<TMP_Text>();

            if (CurrItemStack.Amount > 0)
            {
                text.text = CurrItemStack.Amount.ToString();
            }
            else
            {
                text.text = "";
            }
        }
    }
}
