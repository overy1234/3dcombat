using UnityEngine;

public class ChaseState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        Debug.Log("체이스 Enter 스테이트");
    }

    public override void Execute()
    {
        Debug.Log("체이스 Execute 스테이트");
    }

    public override void Exit()
    {
        Debug.Log("체이스 Exit 스테이트");
    }


}
