using System.Collections;
using UnityEngine;

public class AttackState : State<EnemyController>
{

    [SerializeField] float attackDistance = 1f;

    bool isAttacking;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking)
            return;


        enemy.NavAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= attackDistance + 0.03f)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isAttacking = true;
        enemy.Anim.applyRootMotion = true;

        enemy.Fighter.TryToAttack();
        yield return new WaitUntil(() => enemy.Fighter.attackState == EAttackState.Idle);

        enemy.Anim.applyRootMotion = false;
        isAttacking = false;

    }



}
