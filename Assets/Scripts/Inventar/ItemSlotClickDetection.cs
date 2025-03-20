using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotClickDetection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject draggable;
    private ItemSlotScript itemSlotScript;

    public bool MouseOver = false;

    private Outline outline;

    void Awake()
    {
        itemSlotScript = draggable.GetComponent<ItemSlotScript>();
        outline = GetComponent<Outline>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            itemSlotScript.OnClick(true);
        }
        else
        {
            itemSlotScript.OnClick();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOver = true;
        outline.enabled = true;

        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
        {
            itemSlotScript.OnClick(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOver = false;
        outline.enabled = false;
    }
}
