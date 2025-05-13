using UnityEngine;

public class Combo : MonoBehaviour
{
    Animator playerAnim; // 플레이어 애니메이션

    bool comboPosible; // 콤보 가능 여부
    public int comboStep; // 콤보 단계
    bool inputSmash; // 입력 스매시 여부

    public GameObject hitBox; // 히트박스 오브젝트

    void Start()
    {
        playerAnim = GetComponent<Animator>(); // 플레이어 애니메이션 가져오기
    }

    public void ComboPossible()
    {
        comboPosible = true; // 콤보 가능
    }

    public void NextAtk()
    {
        if(!inputSmash)
        {
            if(comboStep == 2)
            {
                //기본공격격
                 playerAnim.Play("ARPG_Samurai_Attack_Combo3"); // 일반 공격 애니메이션 재생
            }
             if(comboStep == 3)
            {
                 playerAnim.Play("ARPG_Samurai_Attack_Combo4"); // 일반 공격 애니메이션 재생
            }
        }

        if(inputSmash)
        {
            if(comboStep == 1)
            {
                //playerAnim.Play();
            }
             if(comboStep == 2)
            {
                //playerAnim.Play();
            }
             if(comboStep == 3)
            {
                //playerAnim.Play();
            }
        }

    }


    public void ResetCombo()
    {
        comboPosible = false; // 콤보 불가능
        comboStep = 0; // 콤보 단계 초기화
        inputSmash = false; // 입력 스매시 초기화
    }


    void NormaAttack()
    {
        if(comboStep == 0)
        {
            playerAnim.Play("ARPG_Samurai_Attack_Combo2"); // 일반 공격 애니메이션 재생
            comboStep = 1; // 콤보 단계 1
            return;
        }

        if(comboStep !=0)
        {
            if(comboPosible)
            {
                comboPosible = false; // 콤보 불가능
                comboStep += 1; // 콤보 단계 증가
            }
        }
    }


   void SmashAttack()
   {
     if(comboPosible)
     {
        comboPosible = false; // 콤보 불가능
        inputSmash = true; // 입력 스매시
     }
   }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            NormaAttack(); // 일반 공격
        }

        if(Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 클릭
        {
            SmashAttack(); // 스매시 공격
        }

        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            //방어
        }
    }
    
   

    void ChageTag(string t)
    {
        hitBox.tag = t; // 히트박스 태그 변경
    }


}
