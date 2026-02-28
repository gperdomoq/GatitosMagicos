using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class IngredientBattle : MonoBehaviour
{
    [Header("Jugador")]
    public int playerNumber = 1;

    [Header("Ingredientes disponibles")]
    public IngredientData[] ingredients;

    [Header("UI")]
    public TextMeshProUGUI patternDisplay;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI scoreDisplay;

    [Header("Animacion")]
    public Animator ingredientAnimator;

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
    IngredientData currentIngredient;
    int[] currentPattern = new int[4];
    int currentStep = 0;
    float ingredientTimer = 0f;
    float globalTimer = 0f;
    bool battleActive = false;
    bool gameOver = false;

    void OnEnable()
    {
        validKeys = new Key[] { upKey, downKey, leftKey, rightKey };
        keyLabels = new string[] {
            upKey.ToString(),
            downKey.ToString(),
            leftKey.ToString(),
            rightKey.ToString()
        };

        globalTimer = totalTime;
        battleActive = true;
        gameOver = false;
        SpawnNextIngredient();
    }

    void SpawnNextIngredient()
    {
        currentIngredient = ingredients[Random.Range(0, ingredients.Length)];
        SetupAnimator(currentIngredient);

        currentStep = 0;
        ingredientTimer = timePerIngredient;

        for (int i = 0; i < 4; i++)
            currentPattern[i] = Random.Range(0, validKeys.Length);

        UpdatePatternDisplay();
    }

    void SetupAnimator(IngredientData data)
    {
        if (data.idleClip == null) return;

        var controller = new UnityEditor.Animations.AnimatorController();
        controller.AddLayer("Base");
        var sm = controller.layers[0].stateMachine;

        var idle = sm.AddState("Idle");
        var win = sm.AddState("Win");
        var lose = sm.AddState("Lose");

        idle.motion = data.idleClip;
        win.motion = data.winClip;
        lose.motion = data.loseClip;

        controller.AddParameter("win", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("lose", AnimatorControllerParameterType.Trigger);

        var toWin = idle.AddTransition(win);
        toWin.AddCondition(AnimatorConditionMode.If, 0, "win");
        toWin.hasExitTime = false;

        var toLose = idle.AddTransition(lose);
        toLose.AddCondition(AnimatorConditionMode.If, 0, "lose");
        toLose.hasExitTime = false;

        ingredientAnimator.runtimeAnimatorController = controller;
        ingredientAnimator.Play("Idle");
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
            ingredientAnimator.SetTrigger("win");
            return;
        }

        ingredientTimer -= Time.deltaTime;
        if (ingredientTimer <= 0f)
        {
            OnPlayerFail();
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
                        OnPlayerWin();
                }
                else
                {
                    OnPlayerFail();
                }
                break;
            }
        }
    }

    void OnPlayerWin()
    {
        ingredientAnimator.SetTrigger("lose");
        GameManager.Instance.AddIngredient(playerNumber, currentIngredient.ingredientName);
        UpdateScoreDisplay();
        Invoke(nameof(SpawnNextIngredient), 1f);
    }

    void OnPlayerFail()
    {
        ingredientAnimator.SetTrigger("win");
        Invoke(nameof(SpawnNextIngredient), 1f);
    }

    void UpdateScoreDisplay()
    {
        var inv = GameManager.Instance.GetIngredients(playerNumber);
        scoreDisplay.text = $"Ingredientes: {inv.Count}";
    }
}