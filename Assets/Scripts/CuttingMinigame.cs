using UnityEngine;

public class CuttingMinigame : MonoBehaviour
{
    [Header("Sprite a cortar")]
    public SpriteRenderer targetSprite;

    [Header("Clicks necesarios para cortar")]
    public int clicksRequired = 5;

    [Header("Shake")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    [Header("Ingredientes procesados (mismo orden que en el selector)")]
    public string[] processedIngredientNames;

    StationSwitcher stationSwitcher;
    IngredientSelector ingredientSelector;
    int currentClicks = 0;
    bool completed = false;
    Vector3 originalPosition;
    float shakeTimer = 0f;

    void Awake()
    {
        stationSwitcher = GetComponentInParent<StationSwitcher>();
        ingredientSelector = GetComponentInParent<IngredientSelector>();
    }

    void OnEnable()
    {
        currentClicks = 0;
        completed = false;
        if (targetSprite != null)
        {
            originalPosition = targetSprite.transform.localPosition;
            UpdateColor();
        }
    }

    void Update()
    {
        if (completed) return;

        if (stationSwitcher.ActionPressed)
        {
            currentClicks++;
            shakeTimer = shakeDuration;
            UpdateColor();

            if (currentClicks >= clicksRequired)
                CompleteMinigame();
        }

        if (shakeTimer > 0)
        {
            targetSprite.transform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            targetSprite.transform.localPosition = originalPosition;
        }
    }

    void CompleteMinigame()
    {
        completed = true;

        string selected = ingredientSelector.SelectedIngredient;

        for (int i = 0; i < ingredientSelector.validIngredients.Length; i++)
        {
            if (ingredientSelector.validIngredients[i] == selected)
            {
                if (i < processedIngredientNames.Length)
                {
                    GameManager.Instance.AddProcessedIngredient(stationSwitcher.playerNumber, processedIngredientNames[i]);
                    Debug.Log($"¡Ingrediente cortado! Se generó: {processedIngredientNames[i]}");
                }
                break;
            }
        }
    }

    void UpdateColor()
    {
        float progress = (float)currentClicks / clicksRequired;
        targetSprite.color = Color.Lerp(Color.white, Color.red, progress);
    }
}