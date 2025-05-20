using UnityEngine;

public class IdleState : State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        Debug.Log("아이들 Enter 스테이트");
    }

    public override void Execute()
    {
        Debug.Log("아이들 Execute 스테이트");
        if (Input.GetKeyDown(KeyCode.T))
            enemy.ChangeState(EnemyStates.Chase);

    }

    public override void Exit()
    {
        Debug.Log("아이들 Exit 스테이트");
    }

}
