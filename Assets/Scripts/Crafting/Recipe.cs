using System.Collections.Generic;

public class Recipe
{
    public int ID;
    public List<ItemStack> Ingredients;
    public ItemStack Result;

    public Recipe(int id, List<ItemStack> ingredients, ItemStack result)
    {
        ID = id;
        Ingredients = ingredients;
        Result = result;
    }
}
