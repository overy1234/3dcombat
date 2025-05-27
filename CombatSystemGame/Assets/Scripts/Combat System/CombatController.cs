using UnityEngine;

// 전투 시스템을 총괄하는 컨트롤러 클래스
public class CombatController : MonoBehaviour
{
    EnemyController targetEnemy;
    public EnemyController TargetEnemy
    {
        get => targetEnemy;
        set
        {
            targetEnemy = value;

            if (targetEnemy == null)
                CombatMode = false;
        }
    }

    bool combatMode;
    public bool CombatMode 
    {
        get => combatMode;
        set
        {
            combatMode = value;

            if (targetEnemy == null)
                combatMode = false;

            animator.SetBool("combatMode", combatMode);
        }
     
    }

    // 근접 전투 시스템 참조
    MeeleFighter meeleFighter;
    Animator animator;
    CameraController cam;
    private void Awake()
    {
        // 근접 전투 컴포넌트 가져오기
        meeleFighter = GetComponent<MeeleFighter>();
        animator = GetComponent<Animator>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
       
        // 마우스 좌클릭 시 공격 시도
        if (Input.GetMouseButtonDown(0))
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if(enemy != null && enemy.Fighter.IsCounterable && !meeleFighter.inAction)
            {
                StartCoroutine(meeleFighter.PerformCounterAttack(enemy));
            }
            else
            {
                var enemyToAttack =  EnemyManager.i.GetClosestEnemyToDirection(PlayerController.i.InputDir);
                Vector3? dirToAttack = null;
                if (enemyToAttack != null)
                    dirToAttack = enemyToAttack.transform.position - transform.position;


                meeleFighter.TryToAttack(dirToAttack);
                CombatMode = true;
            }               
        }

        if(Input.GetButtonDown("LockOn"))
        {
            CombatMode = !CombatMode;
        }




    }


    private void OnAnimatorMove()
    {
        if (!meeleFighter.InCounter)
        {
            transform.position += animator.deltaPosition;
        }

        transform.rotation *= animator.deltaRotation;
    }


    public Vector3 GetTargetingDir()
    {
        if(!CombatMode)
        {
            var vecFromCam = transform.position - cam.transform.position;
            vecFromCam.y = 0f;
            return vecFromCam.normalized;
        }
        else
        {
            return transform.forward;
        }

     
    }



}
