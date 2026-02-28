using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class IngredientBattle : MonoBehaviour
{
    [Header("Prefabs de ingredientes")]
    public Ingredient[] ingredientPrefabs;

    [Header("Punto de spawn")]
    public Transform spawnPoint;

    [Header("UI")]
    public TextMeshProUGUI patternDisplay;
    public TextMeshProUGUI timerDisplay;

    [Header("Inventario UI")]
    public TextMeshProUGUI[] ingredientCounts;

    [Header("Configuracion")]
    public float timePerIngredient = 5f;
    public float totalTime = 60f;

    [Header("Teclas")]
    public Key upKey = Key.W;
    public Key downKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;

    [Header("Jugador")]
    public int playerNumber = 1;

    Key[] validKeys;
    string[] keyLabels;
    Ingredient currentIngredient;
    GameObject currentIngredientObj;
    int[] currentPattern = new int[4];
    int currentStep = 0;
    float ingredientTimer = 0f;
    float globalTimer = 0f;
    int score = 0;
    bool battleActive = false;
    bool gameOver = false;
    int[] ingredientOrder;
    int orderIndex = 0;

    void ShuffleOrder()
    {
        ingredientOrder = new int[ingredientPrefabs.Length];
        for (int i = 0; i < ingredientPrefabs.Length; i++)
            ingredientOrder[i] = i;

        for (int i = ingredientOrder.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = ingredientOrder[i];
            ingredientOrder[i] = ingredientOrder[rand];
            ingredientOrder[rand] = temp;
        }

        orderIndex = 0;
    }

    Ingredient PickNextIngredient()
    {
        if (orderIndex >= ingredientOrder.Length)
            ShuffleOrder();

        return ingredientPrefabs[ingredientOrder[orderIndex++]];
    }

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
        ShuffleOrder();
        SpawnNextIngredient();
    }

    void OnDisable()
    {
        if (currentIngredientObj != null)
            Destroy(currentIngredientObj);
    }

    void SpawnNextIngredient()
    {
        if (currentIngredientObj != null)
            Destroy(currentIngredientObj);

        Ingredient prefab = PickNextIngredient();
        currentIngredientObj = Instantiate(prefab.gameObject, spawnPoint.position, Quaternion.identity, spawnPoint);
        currentIngredient = currentIngredientObj.GetComponent<Ingredient>();
        currentIngredient.PlayIdle();

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

        if (globalTimer <= 0f)
        {
            gameOver = true;
            patternDisplay.text = "¡Tiempo!";
            if (currentIngredient != null) currentIngredient.PlayWin();
            return;
        }

        ingredientTimer -= Time.deltaTime;
        if (ingredientTimer <= 0f)
        {
            if (currentIngredient != null) currentIngredient.PlayWin();
            SpawnNextIngredient();
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
                        currentIngredient.PlayLose();
                        score++;
                        GameManager.Instance.AddIngredient(playerNumber, currentIngredient.ingredientName);
                        UpdateInventoryUI();
                        SpawnNextIngredient();
                    }
                }
                else
                {
                    if (currentIngredient != null) currentIngredient.PlayWin();
                    SpawnNextIngredient();
                }
                break;
            }
        }
    }

    void UpdateInventoryUI()
    {
        var inv = GameManager.Instance.GetIngredients(playerNumber);

        for (int i = 0; i < ingredientPrefabs.Length; i++)
        {
            if (ingredientCounts[i] == null) continue;

            string name = ingredientPrefabs[i].ingredientName;
            int count = 0;
            foreach (var item in inv)
                if (item == name) count++;

            ingredientCounts[i].text = $"x{count}";
        }
    }
}