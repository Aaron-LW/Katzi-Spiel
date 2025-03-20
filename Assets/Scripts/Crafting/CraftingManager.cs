using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    List<Recipe> InitialisedRecipes = new List<Recipe>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void InitManager()
    {
        InitialisedRecipes.Add(new Recipe(0, new List<ItemStack> { new ItemStack(InventarManager.Instance.InitialisedItems[1], 300) }, new ItemStack(InventarManager.Instance.InitialisedItems[3], 100)));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CraftRecipe(0);
        }
    }

    public void CraftRecipe(int RecipeID)
    {
        Recipe recipe = InitialisedRecipes[RecipeID];

        if (InventarManager.Instance.FindItems(recipe.Ingredients))
        {
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                InventarManager.Instance.RemoveItem(0, recipe.Ingredients[i].Item.ID, recipe.Ingredients[i].Amount, -1);
            }

            InventarManager.Instance.AddItem(0, recipe.Result.Item.ID, recipe.Result.Amount);
        }
        else
        {
            Debug.LogError("Du bist scheiße es funktioniert nicht was dachtest du dir");
        }
    }
}
