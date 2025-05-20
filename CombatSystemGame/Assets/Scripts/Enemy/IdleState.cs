using UnityEngine;

public class IdleState : State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        Debug.Log("���̵� Enter ������Ʈ");
    }

    public override void Execute()
    {
        Debug.Log("���̵� Execute ������Ʈ");
        if (Input.GetKeyDown(KeyCode.T))
            enemy.ChangeState(EnemyStates.Chase);

    }

    public override void Exit()
    {
        Debug.Log("���̵� Exit ������Ʈ");
    }

}
