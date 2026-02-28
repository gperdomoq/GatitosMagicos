using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string ingredientName;
    public Sprite icon;
    public Animator animator;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayIdle() { if (animator != null) animator.Play("idle"); }
    public void PlayWin() { if (animator != null) animator.Play("victory"); }
    public void PlayLose() { if (animator != null) animator.Play("lose"); }
}