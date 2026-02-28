using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class IngredientSelector : MonoBehaviour
{
    [Header("Ingredientes validos para esta estacion")]
    public string[] validIngredients;

    [Header("Sprites de cada ingrediente (mismo orden)")]
    public SpriteRenderer[] ingredientImages;
    public TextMeshProUGUI[] ingredientCountTexts;

    [Header("Paneles")]
    public GameObject selectionPanel;
    public GameObject[] minigamePanels;

    public string SelectedIngredient { get; private set; }

    int playerNumber;
    Key prevKey;
    Key nextKey;
    Key confirmKey;
    int selectedIndex = 0;

    void Awake()
    {
        StationSwitcher switcher = GetComponentInParent<StationSwitcher>();
        playerNumber = switcher.playerNumber;

        if (playerNumber == 1)
        {
            prevKey = Key.W;
            nextKey = Key.S;
            confirmKey = Key.V;
        }
        else
        {
            prevKey = Key.UpArrow;
            nextKey = Key.DownArrow;
            confirmKey = Key.M;
        }
    }

    void OnEnable()
    {
        selectedIndex = 0;
        selectionPanel.SetActive(true);
        foreach (GameObject panel in minigamePanels)
            panel.SetActive(false);
        UpdateSelectionUI();
    }

    void UpdateSelectionUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager no encontrado");
            return;
        }

        for (int i = 0; i < validIngredients.Length; i++)
        {
            if (ingredientImages == null || ingredientImages.Length == 0) { Debug.LogError("ingredientImages vacio"); return; }
            if (ingredientCountTexts == null || ingredientCountTexts.Length == 0) { Debug.LogError("ingredientCountTexts vacio"); return; }
            if (i >= ingredientImages.Length) { Debug.LogError($"ingredientImages falta index {i}"); continue; }
            if (i >= ingredientCountTexts.Length) { Debug.LogError($"ingredientCountTexts falta index {i}"); continue; }
            if (ingredientImages[i] == null) { Debug.LogError($"ingredientImages[{i}] null"); continue; }
            if (ingredientCountTexts[i] == null) { Debug.LogError($"ingredientCountTexts[{i}] null"); continue; }

            int count = GameManager.Instance.GetIngredientCount(playerNumber, validIngredients[i]);
            ingredientCountTexts[i].text = $"x{count}";
            ingredientImages[i].color = i == selectedIndex ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }
    }

    void Update()
    {
        if (!selectionPanel.activeSelf) return;

        if (Keyboard.current[prevKey].wasPressedThisFrame)
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            UpdateSelectionUI();
        }

        if (Keyboard.current[nextKey].wasPressedThisFrame)
        {
            selectedIndex = Mathf.Min(validIngredients.Length - 1, selectedIndex + 1);
            UpdateSelectionUI();
        }

        if (Keyboard.current[confirmKey].wasPressedThisFrame)
        {
            string selected = validIngredients[selectedIndex];
            int count = GameManager.Instance.GetIngredientCount(playerNumber, selected);

            if (count > 0)
            {
                SelectedIngredient = selected;
                GameManager.Instance.UseIngredient(playerNumber, selected);
                selectionPanel.SetActive(false);
                foreach (GameObject panel in minigamePanels)
                    panel.SetActive(true);
            }
            else
            {
                ingredientImages[selectedIndex].color = Color.red;
            }
        }
    }
}