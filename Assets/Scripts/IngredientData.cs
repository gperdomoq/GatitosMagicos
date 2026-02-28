using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Game/Ingredient")]
public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;

    [Header("Animaciones")]
    public AnimationClip idleClip;
    public AnimationClip winClip;
    public AnimationClip loseClip;
}