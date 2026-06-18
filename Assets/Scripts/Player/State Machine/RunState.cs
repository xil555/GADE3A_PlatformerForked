using UnityEngine;

public class RunState : IState
{
    private readonly PlayerController player;

    public RunState(PlayerController player) => this.player = player;

    public void Enter()
    {
        if (player.animator == null) return;
        player.animator.SetBool("IsRunning", true);
        player.animator.SetBool("IsWalking", false);
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

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(new WalkState(player));
            return;
        }
    }

    public void Exit()
    {
        if (player.animator == null) return;
        player.animator.SetBool("IsRunning", false);
        player.animator.SetBool("IsWalking", false);
    }
}
