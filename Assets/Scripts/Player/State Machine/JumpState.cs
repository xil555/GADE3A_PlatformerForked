using UnityEngine;

public class JumpState : IState
{
    private readonly PlayerController player;

    public JumpState(PlayerController player) => this.player = player;

    public void Enter()
    {
        if (player.anim != null)
        {
            player.anim.SetTrigger("Jump");
        }

        player.ConsumeQueuedJump();
    }

    public void Update()
    {
        if (!player.isGrounded) return;
        if (player.playerInputController == null) return;

        Vector2 input = player.playerInputController.MovementInputVector;

        if (input != Vector2.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                player.stateMachine.ChangeState(new RunState(player));
            else
                player.stateMachine.ChangeState(new WalkState(player));
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    public void Exit() { }
}
