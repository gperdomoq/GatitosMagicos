using UnityEngine;
using UnityEditor.Animations;

void SetupAnimator(IngredientData data)
{
    AnimatorController controller = new AnimatorController();
    controller.AddLayer("Base");

    // Agrega los estados
    var stateMachine = controller.layers[0].stateMachine;
    var idleState = stateMachine.AddState("Idle");
    var winState = stateMachine.AddState("Win");
    var loseState = stateMachine.AddState("Lose");

    // Asigna los clips
    idleState.motion = data.idleClip;
    winState.motion = data.winClip;
    loseState.motion = data.loseClip;

    // Agrega parametros de transicion
    controller.AddParameter("win", AnimatorControllerParameterType.Trigger);
    controller.AddParameter("lose", AnimatorControllerParameterType.Trigger);

    // Transiciones
    var toWin = idleState.AddTransition(winState);
    toWin.AddCondition(AnimatorConditionMode.If, 0, "win");
    toWin.hasExitTime = false;

    var toLose = idleState.AddTransition(loseState);
    toLose.AddCondition(AnimatorConditionMode.If, 0, "lose");
    toLose.hasExitTime = false;

    ingredientAnimator.runtimeAnimatorController = controller;
    ingredientAnimator.Play("Idle");
}