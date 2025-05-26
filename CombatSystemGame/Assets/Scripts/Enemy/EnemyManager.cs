using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    [SerializeField] CombatController player;
    public static EnemyManager i {  get; private set; }

    private void Awake()
    {
        i = this; 
    }



    List<EnemyController> enemiesInRange = new List<EnemyController>();
    float notAttackingTimer = 2;

    public void AddEnemyInRange(EnemyController enemy)
    {
        if (!enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy);
    }


    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }



    float timer = 0f;
    private void Update()
    {
        if (enemiesInRange.Count == 0) return;



        if (!enemiesInRange.Any(e => e.IsInState(EnemyStates.Attack)))
        {
            if (notAttackingTimer > 0)
            {
                notAttackingTimer -= Time.deltaTime;
            }

            if (notAttackingTimer <= 0)
            {
                //����
                var attackingEnemy =  SelectenemyForAttack();


                if(attackingEnemy != null)
                {
                    attackingEnemy.ChangeState(EnemyStates.Attack);
                    notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
                }
              
            }
        }

        if(timer >=0.1f)
        {
            timer = 0f;
            var closestEnemy = GetClosesEnemyToPlayerDir();
            if (closestEnemy != null && closestEnemy != player.targetEnemy)
            {
                var prevEnemy = player.targetEnemy;
                player.targetEnemy = closestEnemy;
                player?.targetEnemy?.MeshHighlighter.HighlightMesh(true);
                prevEnemy?.MeshHighlighter.HighlightMesh(false);
            }
        }
      

        timer += Time.deltaTime;
    }

    EnemyController SelectenemyForAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.combatMovementTimer).FirstOrDefault(e => e.Target != null);
    }


    public EnemyController GetAttackingEnemy()
    {
        return enemiesInRange.FirstOrDefault(e => e.IsInState(EnemyStates.Attack));
    }


    public EnemyController GetClosesEnemyToPlayerDir()
    {
        var targetingDir = player.GetTargetingDir();

        float minDinstance = Mathf.Infinity;
        EnemyController closestEnemy = null;

        foreach(var enemy in enemiesInRange)
        {
            var vecToEnemy = enemy.transform.position - player.transform.position;
            vecToEnemy.y = 0;


            float angle  =  Vector3.Angle(targetingDir, vecToEnemy);
            float distance = vecToEnemy.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);

            if(distance < minDinstance)
            {
                minDinstance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }




}
