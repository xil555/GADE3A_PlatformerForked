using UnityEngine;

public class WalkState : IState
{
    private readonly PlayerController player;

    public WalkState(PlayerController player) => this.player = player;

    public void Enter()
    {
        if (player.animator == null) return;
        player.animator.SetBool("IsWalking", true);
        player.animator.SetBool("IsRunning", false);
    }

    public void Update()
    {
        if (player.playerInputController == null) return;

        if (player.jumpQueued)
        {
            player.stateMachine.ChangeState(new JumpState(player));
            return;
        }

        Vector2 input = player.playerInputController.MovementInputVector;

        if (input == Vector2.zero)
        {
            player.stateMachine.ChangeState(new IdleState(player));
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(new RunState(player));
            return;
        }
    }

    public void Exit()
    {
        if (player.animator == null) return;
        player.animator.SetBool("IsWalking", false);
        player.animator.SetBool("IsRunning", false);
    }
}
