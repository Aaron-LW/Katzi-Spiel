using UnityEngine;

public class ItemSlotScript : MonoBehaviour
{
    [HideInInspector] public int CurrItemIndex;
    [HideInInspector] public int CurrInventarIndex;

    public void OnClick(bool quick = false)
    {
        InventarManager.Instance.SwitchItem(CurrInventarIndex, CurrItemIndex, quick);
    }
}
