using System.Collections.Generic;

public class Quest
{
    public int ID;
    public string Name;
    public List<ItemStack> RequiredItems;
    public ItemStack Reward;
    public bool Done;

    public Quest(int id, string name, List<ItemStack> requiredItems, ItemStack reward, bool done = false)
    {
        ID = id;
        Name = name;
        RequiredItems = requiredItems;
        Reward = reward;
        Done = done;
    }
}
