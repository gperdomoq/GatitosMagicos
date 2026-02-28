using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string ingredientName;
    public Sprite icon;
    public Animator animator;

    public void PlayIdle() { if (animator != null) animator.Play("Idle"); }
    public void PlayWin() { if (animator != null) animator.Play("Win"); }
    public void PlayLose() { if (animator != null) animator.Play("Lose"); }
}