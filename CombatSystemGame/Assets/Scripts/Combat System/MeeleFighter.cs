using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum EAttackState { Idle, Windup, Impact,Cooldown}

// 근접 전투를 담당하는 클래스
public class MeeleFighter : MonoBehaviour
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;
    [SerializeField] float rotationSpeed = 500;

    BoxCollider swordCollider;
    SphereCollider leftHandCollider,rightHandCollider,leftFootCollider,rightFootCollider;

    public event Action OnGoHit;
    public event Action OnHitComplete;


    // 애니메이터 컴포넌트 참조
    Animator animator;
    // 현재 공격 동작 중인지 확인하는 플래그
    public bool inAction { get; private set; } = false;
    public bool InCounter { get; set; } = false;

    private void Awake()
    {
        // 애니메이터 컴포넌트 가져오기
        animator = GetComponent<Animator>();
    }


    private void Start()
    {
        if(sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
            leftHandCollider  = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider  = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();

            DisableAllHitBox();

        }
    }




    public EAttackState attackState {get;private set;}
    bool doCombo;
    int comboCount = 0;

    // 공격 시도 함수
    public void TryToAttack(Vector3? attackDir = null)
    {
        // 현재 공격 중이 아닐 때만 새로운 공격 시작
        if(!inAction)
        {
            StartCoroutine(Attack(attackDir));
        }
        else if(attackState == EAttackState.Impact || attackState == EAttackState.Cooldown)
        {
            doCombo = true;
        }
    }

    
    // 공격 동작을 처리하는 코루틴
    IEnumerator Attack(Vector3? attackDir = null)
    {
        // 공격 상태 설정
        inAction = true;
        attackState = EAttackState.Windup;


        if(attackDir != null)
        {
          transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), rotationSpeed * Time.deltaTime);
        }




        // Slash 애니메이션으로 부드럽게 전환 (0.2초 동안)
        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null; //1프레임 null로넘어가기

        // 다음 애니메이션 상태 정보 가져오기
        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;

        while(timer <= animState.length)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / animState.length;

            if(attackState == EAttackState.Windup)
            {

                if (InCounter) break;



                if(normalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    attackState = EAttackState.Impact;
                    //콜라이더 키고
                    EnableHitBox(attacks[comboCount]);
                }
            }
            else if(attackState == EAttackState.Impact)
            {
                if(normalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    attackState = EAttackState.Cooldown;
                    //콜라이더 끄기
                    DisableAllHitBox();
                }
            }
            else if(attackState == EAttackState.Cooldown)
            {
                //콤보
                if(doCombo)
                {
                    doCombo = false;

                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
            }

            
                yield return null;
        }


        attackState = EAttackState.Idle;
        comboCount = 0;
        // 공격 상태 해제
        inAction = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hitbox") && !inAction)
        {
            StartCoroutine(PlayHitReaction(other.GetComponentInParent<MeeleFighter>().transform));
        }
    }

    IEnumerator PlayHitReaction(Transform attacker)
    {
        // 공격 상태 설정
        inAction = true;

        var dispVec = attacker.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);


        OnGoHit?.Invoke();



        // Slash 애니메이션으로 부드럽게 전환 (0.2초 동안)
        animator.CrossFade("SwordImpact", 0.2f);
        yield return null; //1프레임 null로넘어가기

        // 다음 애니메이션 상태 정보 가져오기
        var animState = animator.GetNextAnimatorStateInfo(1);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animState.length *0.8f);

        OnHitComplete?.Invoke();

        // 공격 상태 해제
        inAction = false;
    }


    public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        // 공격 상태 설정
        inAction = true;

        InCounter = true;

        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyStates.Dead);


        var dispVec = opponent.transform.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);
        opponent.transform.rotation = Quaternion.LookRotation(-dispVec);

        var targetPos = opponent.transform.position - dispVec.normalized * 1f;



        animator.CrossFade("CounterAttack", 0.2f);
        opponent.Anim.CrossFade("CounterAttackVictim", 0.2f);
        




        yield return null; //1프레임 null로넘어가기

        // 다음 애니메이션 상태 정보 가져오기
        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while(timer <= animState.length)
        {
           transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);
            yield return null;
            timer += Time.deltaTime;
        }


        InCounter = false;

        opponent.Fighter.InCounter = false;

        // 공격 상태 해제
        inAction = false;
    }



    void EnableHitBox(AttackData attack)
    {
        switch(attack.HitboxToUse)
        {
            case AttackHitbox.LeftHand:
              if (leftHandCollider != null)
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
             if (rightHandCollider != null)
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
             if (leftFootCollider != null)
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
             if (rightFootCollider != null)
                rightFootCollider.enabled = true;
                break;
            case AttackHitbox.Sword:
            if (swordCollider != null)
                swordCollider.enabled = true;
                break;
            default:
                break;
        }
    }






    void DisableAllHitBox()        
    {
        if (swordCollider != null)
            swordCollider.enabled = false;

        if(leftHandCollider != null)
             leftHandCollider.enabled = false;
        if (rightHandCollider != null)
            rightHandCollider.enabled = false;
        if (leftFootCollider != null)
            leftFootCollider.enabled = false;
        if (rightFootCollider != null)
            rightFootCollider.enabled = false;
    }


    public List<AttackData> Attacks => attacks;


    public bool IsCounterable => attackState == EAttackState.Windup && comboCount == 0; 


}
