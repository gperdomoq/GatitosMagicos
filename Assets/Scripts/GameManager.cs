using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<string> player1Ingredients = new List<string>();
    public List<string> player2Ingredients = new List<string>();

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
}