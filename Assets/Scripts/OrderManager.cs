using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Order
{
    public string recipeName;
    public float timeLimit;
    public float timeRemaining;
    public bool completed;
    public bool expired;
}

public class OrderManager : MonoBehaviour
{
    [Header("Configuracion")]
    public int playerNumber = 1;
    public int maxOrders = 5;
    public float minTimeLimit = 30f;
    public float maxTimeLimit = 60f;
    public float newOrderInterval = 15f;

    [Header("UI Slots (5 maximo)")]
    public GameObject[] orderSlots;
    public TextMeshProUGUI[] orderNameTexts;
    public TextMeshProUGUI[] orderTimerTexts;
    public Image[] orderTimerBars;

    List<Order> activeOrders = new List<Order>();
    float nextOrderTimer = 0f;
    int completedCount = 0;

    static readonly string[] recipeNames = { "Viaje Astras", "Megaflopnt", "Te de Calzon" };

    void OnEnable()
    {
        activeOrders.Clear();
        completedCount = 0;
        nextOrderTimer = 0f;
        AddNewOrder();
        UpdateUI();
    }

    void Update()
    {
        // Temporizador para nuevo pedido
        nextOrderTimer -= Time.deltaTime;
        if (nextOrderTimer <= 0f)
        {
            nextOrderTimer = newOrderInterval;
            if (activeOrders.Count < maxOrders)
                AddNewOrder();
        }

        // Actualiza pedidos activos
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = activeOrders[i];
            if (order.completed || order.expired) continue;

            order.timeRemaining -= Time.deltaTime;
            if (order.timeRemaining <= 0f)
            {
                order.expired = true;
                activeOrders.RemoveAt(i);
                Debug.Log($"Pedido expirado: {order.recipeName}");
            }
        }

        UpdateUI();
    }

    void AddNewOrder()
    {
        Order order = new Order();
        order.recipeName = recipeNames[Random.Range(0, recipeNames.Length)];
        order.timeLimit = Random.Range(minTimeLimit, maxTimeLimit);
        order.timeRemaining = order.timeLimit;
        activeOrders.Add(order);
        Debug.Log($"Nuevo pedido: {order.recipeName}");
    }

    public bool TryCompleteOrder(string recipeName)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (activeOrders[i].recipeName == recipeName && !activeOrders[i].completed)
            {
                activeOrders[i].completed = true;
                activeOrders.RemoveAt(i);
                completedCount++;
                Debug.Log($"Pedido completado: {recipeName} Total: {completedCount}");

                if (activeOrders.Count < maxOrders)
                    AddNewOrder();

                return true;
            }
        }
        return false;
    }

    public int GetCompletedCount() => completedCount;

    void UpdateUI()
    {
        for (int i = 0; i < orderSlots.Length; i++)
        {
            if (i < activeOrders.Count)
            {
                orderSlots[i].SetActive(true);
                Order order = activeOrders[i];
                orderNameTexts[i].text = order.recipeName;
                orderTimerTexts[i].text = $"{Mathf.CeilToInt(order.timeRemaining)}s";

                if (orderTimerBars[i] != null)
                    orderTimerBars[i].fillAmount = order.timeRemaining / order.timeLimit;

                // Cambia color segun tiempo restante
                float urgency = order.timeRemaining / order.timeLimit;
                orderTimerTexts[i].color = Color.Lerp(Color.red, Color.green, urgency);
            }
            else
            {
                orderSlots[i].SetActive(false);
            }
        }
    }
}