using UnityEngine;

public class RunState : IState
{
    private readonly PlayerController player;

    public RunState(PlayerController player) => this.player = player;

    public void Enter()
    {
        if (player.anim == null) return;
        player.anim.SetBool("IsRunning", true);
        player.anim.SetBool("IsWalking", false);
    }

    public void Update()
    {
        if (player.playerInputController == null) return;
        Vector2 input = player.playerInputController.MovementInputVector;

        if (input == Vector2.zero)
        {
            player.stateMachine.ChangeState(new IdleState(player));
            return;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(new WalkState(player));
            return;
        }

        if (player.isGrounded && player.jumpQueued)
        {
            player.stateMachine.ChangeState(new JumpState(player));
        }
    }

    public void Exit()
    {
        if (player.anim == null) return;
        player.anim.SetBool("IsRunning", false);
    }
}
