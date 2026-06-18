using UnityEngine;

public class JumpState : IState
{
    private readonly PlayerController player;
    private bool leftGround;

    public JumpState(PlayerController player) => this.player = player;

    public void Enter()
    {
        leftGround = false;
        player.PlayJumpAnimation();
        player.ConsumeQueuedJump();
    }

    public void Update()
    {
        if (player.playerInputController == null)
        {
            return;
        }

        if (!player.isGrounded)
        {
            leftGround = true;
            return;
        }

        if (!leftGround)
        {
            return;
        }

        Vector2 input = player.playerInputController.MovementInputVector;

        if (input != Vector2.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                player.stateMachine.ChangeState(new RunState(player));
            }
            else
            {
                player.stateMachine.ChangeState(new WalkState(player));
            }
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    public void Exit() { }
}
