using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class IngredientSelector : MonoBehaviour
{
    [Header("Ingredientes validos para esta estacion")]
    public string[] validIngredients;

    [Header("Indica si cada ingrediente es procesado (mismo orden)")]
    public bool[] isProcessed;

    [Header("Sprites de cada ingrediente (mismo orden)")]
    public SpriteRenderer[] ingredientImages;
    public TextMeshProUGUI[] ingredientCountTexts;

    [Header("Paneles")]
    public GameObject selectionPanel;
    public GameObject[] minigamePanels;

    [Header("Seleccion multiple (para Hervir)")]
    public bool multiSelect = false;
    public int requiredSelections = 3;

    [Header("No consumir al seleccionar (para Hervir)")]
    public bool consumeOnSelect = true;

    public string SelectedIngredient { get; private set; }
    public string[] SelectedIngredients { get; private set; }

    int playerNumber;
    Key prevKey;
    Key nextKey;
    Key confirmKey;
    int selectedIndex = 0;
    bool[] selectedSlots;

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
        selectedSlots = new bool[validIngredients.Length];
        SelectedIngredients = new string[0];
        selectionPanel.SetActive(true);
        foreach (GameObject panel in minigamePanels)
            panel.SetActive(false);
        UpdateSelectionUI();
    }

    bool IsProcessed(int index)
    {
        return isProcessed != null && index < isProcessed.Length && isProcessed[index];
    }

    int GetCount(int index)
    {
        return IsProcessed(index)
            ? GameManager.Instance.GetProcessedIngredientCount(playerNumber, validIngredients[index])
            : GameManager.Instance.GetIngredientCount(playerNumber, validIngredients[index]);
    }

    void UpdateSelectionUI()
    {
        if (GameManager.Instance == null) { Debug.LogWarning("GameManager no encontrado"); return; }

        for (int i = 0; i < validIngredients.Length; i++)
        {
            if (ingredientImages == null || ingredientImages.Length == 0) { Debug.LogError("ingredientImages vacio"); return; }
            if (ingredientCountTexts == null || ingredientCountTexts.Length == 0) { Debug.LogError("ingredientCountTexts vacio"); return; }
            if (i >= ingredientImages.Length) { Debug.LogError($"ingredientImages falta index {i}"); continue; }
            if (i >= ingredientCountTexts.Length) { Debug.LogError($"ingredientCountTexts falta index {i}"); continue; }
            if (ingredientImages[i] == null) { Debug.LogError($"ingredientImages[{i}] null"); continue; }
            if (ingredientCountTexts[i] == null) { Debug.LogError($"ingredientCountTexts[{i}] null"); continue; }

            int count = GetCount(i);
            ingredientCountTexts[i].text = $"x{count}";

            if (multiSelect)
            {
                if (selectedSlots[i])
                    ingredientImages[i].color = Color.green;
                else if (i == selectedIndex)
                    ingredientImages[i].color = Color.white;
                else
                    ingredientImages[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                ingredientImages[i].color = i == selectedIndex ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    int CountSelected()
    {
        int count = 0;
        foreach (bool s in selectedSlots)
            if (s) count++;
        return count;
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
            if (multiSelect)
                HandleMultiSelect();
            else
                HandleSingleSelect();
        }
    }

    void HandleSingleSelect()
    {
        string selected = validIngredients[selectedIndex];
        int count = GetCount(selectedIndex);

        if (count > 0)
        {
            SelectedIngredient = selected;
            if (consumeOnSelect)
            {
                if (IsProcessed(selectedIndex))
                    GameManager.Instance.UseProcessedIngredient(playerNumber, selected);
                else
                    GameManager.Instance.UseIngredient(playerNumber, selected);
            }
            selectionPanel.SetActive(false);
            foreach (GameObject panel in minigamePanels)
                panel.SetActive(true);
        }
        else
        {
            ingredientImages[selectedIndex].color = Color.red;
        }
    }

    void HandleMultiSelect()
    {
        if (CountSelected() >= requiredSelections)
        {
            ConfirmMultiSelect();
            return;
        }

        int count = GetCount(selectedIndex);
        if (!selectedSlots[selectedIndex] && count > 0)
        {
            selectedSlots[selectedIndex] = true;
            Debug.Log($"Seleccionado: {validIngredients[selectedIndex]}");
        }
        else if (selectedSlots[selectedIndex])
        {
            selectedSlots[selectedIndex] = false;
            Debug.Log($"Deseleccionado: {validIngredients[selectedIndex]}");
        }
        else
        {
            ingredientImages[selectedIndex].color = Color.red;
        }

        UpdateSelectionUI();

        if (CountSelected() >= requiredSelections)
            ConfirmMultiSelect();
    }

    void ConfirmMultiSelect()
    {
        var selected = new System.Collections.Generic.List<string>();

        for (int i = 0; i < validIngredients.Length; i++)
        {
            if (selectedSlots[i])
            {
                selected.Add(validIngredients[i]);
                if (consumeOnSelect)
                {
                    if (IsProcessed(i))
                        GameManager.Instance.UseProcessedIngredient(playerNumber, validIngredients[i]);
                    else
                        GameManager.Instance.UseIngredient(playerNumber, validIngredients[i]);
                }
            }
        }

        SelectedIngredients = selected.ToArray();
        selectionPanel.SetActive(false);
        foreach (GameObject panel in minigamePanels)
            panel.SetActive(true);
    }
}