using UnityEngine;

public class IdleState : IState
{
    private readonly PlayerController player;

    public IdleState(PlayerController player) => this.player = player;

    public void Enter()
    {
        if (player.anim == null) return;
        player.anim.SetBool("IsWalking", false);
        player.anim.SetBool("IsRunning", false);
    }

    public void Update()
    {
        if (player.playerInputController == null) return;
        Vector2 input = player.playerInputController.MovementInputVector;

        if (input != Vector2.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                player.stateMachine.ChangeState(new RunState(player));
            else
                player.stateMachine.ChangeState(new WalkState(player));
            return;
        }

        if (player.isGrounded && player.jumpQueued)
        {
            player.stateMachine.ChangeState(new JumpState(player));
        }
    }

    public void Exit() { }
}
