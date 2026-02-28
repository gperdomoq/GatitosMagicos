using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<string> player1Ingredients = new List<string>();
    public List<string> player2Ingredients = new List<string>();
    public List<string> player1ProcessedIngredients = new List<string>();
    public List<string> player2ProcessedIngredients = new List<string>();
    public List<string> player1CompletedPotions = new List<string>();
    public List<string> player2CompletedPotions = new List<string>();

    // Definicion de recetas
    public static readonly Dictionary<string, Dictionary<string, int>> Recipes = new Dictionary<string, Dictionary<string, int>>
    {
        {
            "Viaje Astras", new Dictionary<string, int>
            {
                { "Fumon", 2 },       // catnip procesado
                { "Hongozo", 1 },     // hongo procesado
                { "Lagrimas", 1 }     // original
            }
        },
        {
            "Megaflopnt", new Dictionary<string, int>
            {
                { "Mangazo", 1 },     // mango procesado
                { "Lagrimas", 1 },    // original
                { "Hongozo", 1 }      // hongo procesado
            }
        },
        {
            "Te de Calzon", new Dictionary<string, int>
            {
                { "Lagrimas", 1 },    // original
                { "Mangazo", 1 },     // mango procesado
                { "Lirozo", 1 }       // lirio procesado
            }
        }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddIngredient(int player, string ingredient)
    {
        if (player == 1) player1Ingredients.Add(ingredient);
        else if (player == 2) player2Ingredients.Add(ingredient);
        Debug.Log($"Jugador {player} obtuvo: {ingredient}. Total: {GetIngredients(player).Count}");
    }

    public List<string> GetIngredients(int player)
    {
        return player == 1 ? player1Ingredients : player2Ingredients;
    }

    public int GetIngredientCount(int player, string ingredient)
    {
        int count = 0;
        foreach (var item in GetIngredients(player))
            if (item == ingredient) count++;
        return count;
    }

    public bool UseIngredient(int player, string ingredient)
    {
        List<string> inv = GetIngredients(player);
        if (inv.Contains(ingredient))
        {
            inv.Remove(ingredient);
            return true;
        }
        return false;
    }

    public void AddProcessedIngredient(int player, string ingredient)
    {
        if (player == 1) player1ProcessedIngredients.Add(ingredient);
        else if (player == 2) player2ProcessedIngredients.Add(ingredient);
        Debug.Log($"Jugador {player} procesó: {ingredient}");
    }

    public int GetProcessedIngredientCount(int player, string ingredient)
    {
        List<string> inv = player == 1 ? player1ProcessedIngredients : player2ProcessedIngredients;
        int count = 0;
        foreach (var item in inv)
            if (item == ingredient) count++;
        return count;
    }

    public bool UseProcessedIngredient(int player, string ingredient)
    {
        List<string> inv = player == 1 ? player1ProcessedIngredients : player2ProcessedIngredients;
        if (inv.Contains(ingredient))
        {
            inv.Remove(ingredient);
            return true;
        }
        return false;
    }

    public void ClearIngredients(int player)
    {
        if (player == 1) player1Ingredients.Clear();
        else player2Ingredients.Clear();
    }

    public bool CanMakeRecipe(int player, string recipeName)
    {
        if (!Recipes.ContainsKey(recipeName)) return false;

        foreach (var ingredient in Recipes[recipeName])
        {
            string name = ingredient.Key;
            int required = ingredient.Value;

            // Busca primero en procesados, luego en originales
            int processedCount = GetProcessedIngredientCount(player, name);
            int originalCount = GetIngredientCount(player, name);
            int total = processedCount + originalCount;

            if (total < required)
            {
                Debug.Log($"Falta {name}: tiene {total}, necesita {required}");
                return false;
            }
        }
        return true;
    }

    public bool CompleteRecipe(int player, string recipeName)
    {
        if (!CanMakeRecipe(player, recipeName)) return false;

        foreach (var ingredient in Recipes[recipeName])
        {
            string name = ingredient.Key;
            int required = ingredient.Value;

            for (int i = 0; i < required; i++)
            {
                // Intenta usar procesado primero, si no usa original
                if (!UseProcessedIngredient(player, name))
                    UseIngredient(player, name);
            }
        }

        if (player == 1) player1CompletedPotions.Add(recipeName);
        else player2CompletedPotions.Add(recipeName);

        Debug.Log($"Jugador {player} completó: {recipeName}!");
        return true;
    }

    public List<string> GetCompletedPotions(int player)
    {
        return player == 1 ? player1CompletedPotions : player2CompletedPotions;
    }
}