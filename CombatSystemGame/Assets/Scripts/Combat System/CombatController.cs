using UnityEngine;

// 전투 시스템을 총괄하는 컨트롤러 클래스
public class CombatController : MonoBehaviour
{
    // 근접 전투 시스템 참조
    MeeleFighter meeleFighter;

    private void Awake()
    {
        // 근접 전투 컴포넌트 가져오기
        meeleFighter = GetComponent<MeeleFighter>();
    }

    private void Update()
    {
       
        // 마우스 좌클릭 시 공격 시도
        if (Input.GetMouseButtonDown(0))
        {
            meeleFighter.TryToAttack();
        }
    }
}
