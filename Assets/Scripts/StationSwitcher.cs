using UnityEngine;
using UnityEngine.InputSystem;

public class StationSwitcher : MonoBehaviour
{
    [Header("Stations in order: Libro, Cortar, Revolver, Hervir, Entregar")]
    public GameObject[] stations;

    [Header("Navegacion")]
    public Key prevKey = Key.A;
    public Key nextKey = Key.D;

    [Header("Accion")]
    public Key actionKey = Key.V;

    [Header("Start")]
    public int startIndex = 0;

    [Header("Jugador")]
public int playerNumber = 1;

    int currentIndex;

    public bool ActionPressed => Keyboard.current[actionKey].wasPressedThisFrame;

    void Start()
    {
        currentIndex = Mathf.Clamp(startIndex, 0, stations.Length - 1);
        ShowOnly(currentIndex);
    }

    void Update()
    {
        if (Keyboard.current[prevKey].wasPressedThisFrame) Change(-1);
        if (Keyboard.current[nextKey].wasPressedThisFrame) Change(+1);
    }

    void Change(int dir)
    {
        int target = Mathf.Clamp(currentIndex + dir, 0, stations.Length - 1);
        if (target == currentIndex) return;
        currentIndex = target;
        ShowOnly(currentIndex);
    }

    void ShowOnly(int index)
    {
        for (int i = 0; i < stations.Length; i++)
            stations[i].SetActive(i == index);
    }
}