using UnityEngine;

public class StirringMinigame : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer track;
    public SpriteRenderer movingBar;

    [Header("Limites de movimiento")]
    public float minX = -2f;
    public float maxX = 2f;

    [Header("Apariencia de la linea objetivo")]
    public Color targetColor = Color.red;
    public Vector3 targetScale = new Vector3(0.1f, 1f, 1f);

    [Header("Velocidad")]
    public float moveSpeed = 2f;
    public float speedIncreasePerHit = 0.3f;

    [Header("Aciertos necesarios para ganar")]
    public int hitsRequired = 5;

    [Header("Ingredientes procesados (mismo orden que en el selector)")]
    public string[] processedIngredientNames;

    StationSwitcher stationSwitcher;
    IngredientSelector ingredientSelector;
    SpriteRenderer targetLine;
    float currentX = 0f;
    float direction = 1f;
    float targetX = 0f;
    int currentHits = 0;
    bool completed = false;

    void Awake()
    {
        stationSwitcher = GetComponentInParent<StationSwitcher>();
        ingredientSelector = GetComponentInParent<IngredientSelector>();
        CreateTargetLine();
    }

    void CreateTargetLine()
    {
        GameObject lineObj = new GameObject("TargetLine");
        lineObj.transform.SetParent(movingBar.transform.parent);
        lineObj.transform.localScale = targetScale;
        lineObj.transform.localPosition = Vector3.zero;

        targetLine = lineObj.AddComponent<SpriteRenderer>();
        targetLine.sprite = movingBar.sprite;
        targetLine.color = targetColor;
        targetLine.sortingOrder = movingBar.sortingOrder + 1;
    }

    void OnEnable()
    {
        currentX = minX;
        direction = 1f;
        currentHits = 0;
        completed = false;
        PlaceNewTarget();
    }

    void Update()
    {
        if (completed || movingBar == null) return;

        currentX += direction * (moveSpeed + speedIncreasePerHit * currentHits) * Time.deltaTime;

        if (currentX >= maxX) { currentX = maxX; direction = -1f; }
        else if (currentX <= minX) { currentX = minX; direction = 1f; }

        Vector3 pos = movingBar.transform.localPosition;
        pos.x = currentX;
        movingBar.transform.localPosition = pos;

        if (stationSwitcher.ActionPressed)
        {
            float barHalfWidth = movingBar.bounds.extents.x;
            float barLeft = currentX - barHalfWidth;
            float barRight = currentX + barHalfWidth;

            if (targetX >= barLeft && targetX <= barRight)
            {
                currentHits++;
                Debug.Log($"¡Acierto! {currentHits}/{hitsRequired}");

                if (currentHits >= hitsRequired)
                    CompleteMinigame();
                else
                    PlaceNewTarget();
            }
            else
            {
                Debug.Log("¡Fallaste!");
            }
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
                    Debug.Log($"¡Pocion revuelta! Se generó: {processedIngredientNames[i]}");
                }
                break;
            }
        }
    }

    void PlaceNewTarget()
    {
        targetX = Random.Range(minX, maxX);
        Vector3 targetPos = targetLine.transform.localPosition;
        targetPos.x = targetX;
        targetLine.transform.localPosition = targetPos;
    }
}