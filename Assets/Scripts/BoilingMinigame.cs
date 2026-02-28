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
    float currentX = 0f;
    float driftDirection = 1f;
    float timeInCenter = 0f;
    bool completed = false;

    void Awake()
    {
        stationSwitcher = GetComponentInParent<StationSwitcher>();
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

        // Deriva sola hacia un lado
        currentX += driftDirection * driftSpeed * Time.deltaTime;

        // Cambia de direccion aleatoriamente al llegar al borde
        if (currentX >= maxX)
        {
            currentX = maxX;
            PickNewDriftDirection();
        }
        else if (currentX <= minX)
        {
            currentX = minX;
            PickNewDriftDirection();
        }

        // Click empuja hacia el centro
        if (stationSwitcher.ActionPressed)
        {
            currentX -= Mathf.Sign(currentX) * pushAmount;
        }

        UpdatePosition();
        UpdateColor();

        // Cuenta el tiempo que pasa centrado
        if (Mathf.Abs(currentX) <= safeZone)
        {
            timeInCenter += Time.deltaTime;
            if (timeInCenter >= timeRequired)
            {
                completed = true;
                Debug.Log("¡Pocion hervida!");
            }
        }
        else
        {
            timeInCenter = 0f;
        }
    }

    void PickNewDriftDirection()
    {
        // Elige aleatoriamente izquierda o derecha pero tendiendo a alejarse del centro
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