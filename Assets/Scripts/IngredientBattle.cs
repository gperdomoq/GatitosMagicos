using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class IngredientBattle : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI patternDisplay;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI scoreDisplay;

    [Header("Configuracion")]
    public float timePerIngredient = 5f;
    public float totalTime = 60f;

    [Header("Teclas")]
    public Key upKey = Key.W;
    public Key downKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;

    Key[] validKeys;
    string[] keyLabels;

    int[] currentPattern = new int[4];
    int currentStep = 0;
    float ingredientTimer = 0f;
    float globalTimer = 0f;
    int score = 0;
    bool battleActive = false;
    bool gameOver = false;

    [Header("Jugador")]
    public int playerNumber = 1;

    [Header("Ingrediente actual")]
    public string ingredientName = "Hierba";

    void OnEnable()
    {
        validKeys = new Key[] { upKey, downKey, leftKey, rightKey };
        keyLabels = new string[] {
            upKey.ToString(),
            downKey.ToString(),
            leftKey.ToString(),
            rightKey.ToString()
        };

        score = 0;
        globalTimer = totalTime;
        battleActive = true;
        gameOver = false;
        GeneratePattern();
    }

    void GeneratePattern()
    {
        currentStep = 0;
        ingredientTimer = timePerIngredient;

        for (int i = 0; i < 4; i++)
            currentPattern[i] = Random.Range(0, validKeys.Length);

        UpdatePatternDisplay();
    }

    void UpdatePatternDisplay()
    {
        string display = "";
        for (int i = 0; i < 4; i++)
        {
            if (i == currentStep)
                display += $"<color=yellow>[{keyLabels[currentPattern[i]]}]</color> ";
            else if (i < currentStep)
                display += $"<color=green>{keyLabels[currentPattern[i]]}</color> ";
            else
                display += $"{keyLabels[currentPattern[i]]} ";
        }
        patternDisplay.text = display;
    }

    void Update()
    {
        if (gameOver || !battleActive) return;

        globalTimer -= Time.deltaTime;
        timerDisplay.text = $"Tiempo: {Mathf.CeilToInt(globalTimer)}";
        scoreDisplay.text = $"Ingredientes: {score}";

        if (globalTimer <= 0f)
        {
            gameOver = true;
            patternDisplay.text = "¡Tiempo!";
            Debug.Log($"Fin! Ingredientes: {score}");
            return;
        }

        ingredientTimer -= Time.deltaTime;
        if (ingredientTimer <= 0f)
        {
            Debug.Log("¡Tiempo agotado! Siguiente ingrediente.");
            GeneratePattern();
            return;
        }

        for (int i = 0; i < validKeys.Length; i++)
        {
            if (Keyboard.current[validKeys[i]].wasPressedThisFrame)
            {
                if (i == currentPattern[currentStep])
                {
                    currentStep++;
                    UpdatePatternDisplay();

                    if (currentStep >= 4)
                    {
                        score++;
                        GeneratePattern();
                        GameManager.Instance.AddIngredient(playerNumber, ingredientName);
                        Debug.Log($"¡Ingrediente ganado! Total: {score}");
                    }
                }
                else
                {
                    Debug.Log("¡Fallo! Siguiente ingrediente.");
                    GeneratePattern();
                }
                break;
            }
        }
    }
}