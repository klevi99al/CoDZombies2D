using UnityEngine;

public class PlayerSpriteReset : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement movement = animator.transform.GetComponentInParent<PlayerMovement>();
        movement.isSliding = false;
        animator.SetBool("Slide", false);
        movement.SetPlayerSlideVariables(true);

        if (!StaticVariables.isSoloGame)
        {
            NetworkedVariables networkedVariables = animator.transform.GetComponentInParent<NetworkedVariables>();
            networkedVariables.SetSlidingState(false);
        }
        Debug.Log("Slide animation ended. Resetting sprites.");
    }
}
