using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<Quest> Quests = new List<Quest>();

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

    public void InitManager()
    {
        Quests.Add(new Quest(0, "Collect 300 Wood", new List<ItemStack> { new ItemStack(InventarManager.Instance.InitialisedItems[1], 300) }, new ItemStack(InventarManager.Instance.InitialisedItems[3], 100)));
    }

    public void FinishQuest(Quest quest)
    {
        if (quest.Done)
        {
            Debug.LogError("Quest already done!");
            return;
        }

        if (InventarManager.Instance.FindItems(quest.RequiredItems))
        {
            for (int i = 0; i < quest.RequiredItems.Count; i++)
            {
                InventarManager.Instance.RemoveItem(0, quest.RequiredItems[i].Item.ID, quest.RequiredItems[i].Amount, -1, false);
            }

            InventarManager.Instance.AddItem(0, quest.Reward.Item.ID, quest.Reward.Amount);

            InventarUI.Instance.UpdateInventory(InventarManager.Instance.Inventare[0]);
            InventarUI.Instance.UpdateInventory(InventarManager.Instance.Inventare[1]);

            quest.Done = true;

            return;
        }
    }
}
