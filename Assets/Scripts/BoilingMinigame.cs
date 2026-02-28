using UnityEngine;

public class BoilingMinigame : MonoBehaviour
{
    [Header("Sprite del indicador")]
    public SpriteRenderer indicator;

    [Header("Limites de movimiento")]
    public float minX = -2f;
    public float maxX = 2f;

    [Header("Balance")]
    public float driftSpeed = 1f;
    public float pushAmount = 0.3f;

    [Header("Tiempo para ganar")]
    public float timeRequired = 5f;
    public float safeZone = 0.5f;

    StationSwitcher stationSwitcher;
    int playerNumber;
    float currentX = 0f;
    float driftDirection = 1f;
    float timeInCenter = 0f;
    bool completed = false;

    void Awake()
    {
        stationSwitcher = GetComponentInParent<StationSwitcher>();
        playerNumber = stationSwitcher.playerNumber;
    }

    void OnEnable()
    {
        currentX = 0f;
        timeInCenter = 0f;
        completed = false;
        PickNewDriftDirection();
        UpdatePosition();
    }

    void Update()
    {
        if (completed || indicator == null) return;

        currentX += driftDirection * driftSpeed * Time.deltaTime;

        if (currentX >= maxX) { currentX = maxX; PickNewDriftDirection(); }
        else if (currentX <= minX) { currentX = minX; PickNewDriftDirection(); }

        if (stationSwitcher.ActionPressed)
            currentX -= Mathf.Sign(currentX) * pushAmount;

        UpdatePosition();
        UpdateColor();

        if (Mathf.Abs(currentX) <= safeZone)
        {
            timeInCenter += Time.deltaTime;
            if (timeInCenter >= timeRequired)
            {
                completed = true;
                CompletePotion();
            }
        }
        else
        {
            timeInCenter = 0f;
        }
    }

    void CompletePotion()
    {
        string detectedRecipe = DetectRecipe();

        if (detectedRecipe != null && GameManager.Instance.CompleteRecipe(playerNumber, detectedRecipe))
        {
            OrderManager orderManager = GetComponentInParent<OrderManager>();
            if (orderManager != null)
                orderManager.TryCompleteOrder(detectedRecipe);
            Debug.Log($"¡Pocion completada: {detectedRecipe}!");
        }
        else
        {
            Debug.Log("No coincide con ninguna receta o faltan ingredientes");
        }
    }

    string DetectRecipe()
    {
        Debug.Log("=== Verificando recetas ===");
        Debug.Log($"Originales P{playerNumber}: {string.Join(", ", GameManager.Instance.GetIngredients(playerNumber))}");
        Debug.Log($"Procesados P{playerNumber}: {string.Join(", ", playerNumber == 1 ? GameManager.Instance.player1ProcessedIngredients : GameManager.Instance.player2ProcessedIngredients)}");

        foreach (var recipe in GameManager.Recipes)
        {
            Debug.Log($"Verificando: {recipe.Key}");
            bool can = GameManager.Instance.CanMakeRecipe(playerNumber, recipe.Key);
            Debug.Log($"Puede hacer {recipe.Key}: {can}");
            if (can) return recipe.Key;
        }
        return null;
    }

    void PickNewDriftDirection()
    {
        driftDirection = currentX >= 0 ? 1f : -1f;
        if (Random.value < 0.3f) driftDirection *= -1f;
    }

    void UpdatePosition()
    {
        Vector3 pos = indicator.transform.localPosition;
        pos.x = currentX;
        indicator.transform.localPosition = pos;
    }

    void UpdateColor()
    {
        float distanceFromCenter = Mathf.Abs(currentX) / maxX;
        indicator.color = Color.Lerp(Color.green, Color.red, distanceFromCenter);
    }
}