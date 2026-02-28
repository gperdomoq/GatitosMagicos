using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Intro 2D con Sprites.
/// Cada slide puede tener MÚLTIPLES líneas de texto que aparecen una por una
/// con un delay entre cada una.
/// </summary>
public class IntroSlideshow2D : MonoBehaviour
{
    [System.Serializable]
    public class LineaDeTexto
    {
        [TextArea(2, 4)]
        public string texto;
        public float  esperarAntes = 1.5f;  // segundos antes de que aparezca esta línea
    }

    [System.Serializable]
    public class Slide
    {
        public GameObject    objeto;         // el GameObject de esta slide
        public LineaDeTexto[] lineas;        // las líneas que aparecen una por una
        public float          duracionFinal = 2f; // segundos extra al final antes de pasar
    }

    [Header("Slides")]
    public Slide[] slides;

    [Header("Escena siguiente")]
    public string escenaSiguiente = "BattleScene";

    [Header("Fade entre slides")]
    public float duracionFade = 0.5f;

    private bool _saltando = false;

    // ── Unity ────────────────────────────────────────────────────────────────
    void Start()
    {
        foreach (var s in slides)
            if (s.objeto != null) s.objeto.SetActive(false);

        StartCoroutine(CorrerSlides());
    }

    void Update()
    {
        if (!_saltando && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
            StartCoroutine(Saltar());
    }

    // ── Flujo ────────────────────────────────────────────────────────────────
    private IEnumerator CorrerSlides()
    {
        for (int i = 0; i < slides.Length; i++)
        {
            if (_saltando) yield break;
            yield return StartCoroutine(MostrarSlide(slides[i]));
        }
        if (!_saltando) yield return StartCoroutine(Terminar());
    }

    private IEnumerator MostrarSlide(Slide slide)
    {
        if (slide.objeto == null) yield break;

        // Obtener todos los TextMeshPro de esta slide y ocultarlos
        var textos = slide.objeto.GetComponentsInChildren<TextMeshPro>();
        var fondo  = slide.objeto.GetComponentInChildren<SpriteRenderer>();

        // Activar el objeto con todo invisible
        slide.objeto.SetActive(true);
        foreach (var t in textos) SetAlpha(t, 0f);
        if (fondo != null) SetAlphaSprite(fondo, 0f);

        // Fade in del fondo
        yield return StartCoroutine(FadeSprite(fondo, 0f, 1f));

        // Mostrar líneas una por una
        // Cada TextMeshPro hijo = una línea (en el orden que están en la Hierarchy)
        for (int i = 0; i < textos.Length; i++)
        {
            if (_saltando) yield break;

            // Esperar el delay de esta línea
            float espera = (slide.lineas != null && i < slide.lineas.Length)
                ? slide.lineas[i].esperarAntes
                : 1.5f;

            yield return new WaitForSeconds(espera);
            if (_saltando) yield break;

            // Aparecer con fade in rápido
            yield return StartCoroutine(FadeTexto(textos[i], 0f, 1f, 0.4f));
        }

        // Esperar un poco al final antes de cambiar de slide
        yield return new WaitForSeconds(slide.duracionFinal);

        // Fade out de todo
        foreach (var t in textos) yield return StartCoroutine(FadeTexto(t, 1f, 0f, 0.3f));
        yield return StartCoroutine(FadeSprite(fondo, 1f, 0f));

        slide.objeto.SetActive(false);
    }

    // ── Fades ────────────────────────────────────────────────────────────────
    private IEnumerator FadeTexto(TextMeshPro tm, float desde, float hasta, float duracion)
    {
        if (tm == null) yield break;
        float t = 0f;
        Color c = tm.color;
        while (t < duracion)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracion);
            tm.color = c;
            yield return null;
        }
        c.a = hasta;
        tm.color = c;
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float desde, float hasta)
    {
        if (sr == null) yield break;
        float t = 0f;
        Color c = sr.color;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracionFade);
            sr.color = c;
            yield return null;
        }
        c.a = hasta;
        sr.color = c;
    }

    private void SetAlpha(TextMeshPro tm, float a)
    {
        if (tm == null) return;
        Color c = tm.color; c.a = a; tm.color = c;
    }

    private void SetAlphaSprite(SpriteRenderer sr, float a)
    {
        if (sr == null) return;
        Color c = sr.color; c.a = a; sr.color = c;
    }

    private IEnumerator Saltar()
    {
        _saltando = true;
        yield return StartCoroutine(Terminar());
    }

    private IEnumerator Terminar()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(escenaSiguiente);
    }
}