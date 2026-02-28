using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<string> player1Ingredients = new List<string>();
    public List<string> player2Ingredients = new List<string>();
    public List<string> player1ProcessedIngredients = new List<string>();
    public List<string> player2ProcessedIngredients = new List<string>();

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

    public void ClearIngredients(int player)
    {
        if (player == 1) player1Ingredients.Clear();
        else player2Ingredients.Clear();
    }

    public bool UseIngredient(int player, string ingredient)
    {
        List<string> inv = GetIngredients(player);
        if (inv.Contains(ingredient))
        {
            inv.Remove(ingredient);
            Debug.Log($"Jugador {player} usó: {ingredient}");
            return true;
        }
        Debug.Log($"Jugador {player} no tiene: {ingredient}");
        return false;
    }
}