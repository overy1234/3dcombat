using UnityEngine;

[RequireComponent(typeof(Animator))]
public class KatanaDualHandIK : MonoBehaviour
{
    public Transform rightHandIKTarget;   // 오른손 IK 목표 (카타나 끝)
    public Transform leftHandIKTarget;    // 왼손 IK 목표 (카타나 중간)

    [Range(0f, 1f)] public float rightHandIKWeight = 1f;
    [Range(0f, 1f)] public float leftHandIKWeight = 0f; // 처음에는 0

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null)
            return;

        // 오른손 IK
        if (rightHandIKTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTarget.rotation);
        }

        // 왼손 IK
        if (leftHandIKTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
        }
    }
}




/*
 *
 좋아요, 이번엔 **오른손에도 IK를 사용하고**, 나중에 **양손 모두 IK로 무기에 붙이는 상황**을 다뤄볼게요. 예를 들어,

> 처음에는 **오른손만 카타나를 쥐고**, 이후에는 **왼손도 붙는 애니메이션이 나오는 구조**를 상정하면,

오른손과 왼손 모두 **IK로 제어할 수 있도록** 세팅해줘야 합니다.

---

## ✅ 필요한 구조

```plaintext
Katana (무기 프리팹)
├── RightHandIKTarget (카타나의 손잡이 끝)
└── LeftHandIKTarget  (카타나의 손잡이 중간)
```

---

## 🧠 확장된 IK 스크립트 예제

```csharp
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class KatanaDualHandIK : MonoBehaviour
{
    public Transform rightHandIKTarget;   // 오른손 IK 목표 (카타나 끝)
    public Transform leftHandIKTarget;    // 왼손 IK 목표 (카타나 중간)

    [Range(0f, 1f)] public float rightHandIKWeight = 1f;
    [Range(0f, 1f)] public float leftHandIKWeight = 0f; // 처음에는 0

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null)
            return;

        // 오른손 IK
        if (rightHandIKTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTarget.rotation);
        }

        // 왼손 IK
        if (leftHandIKTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
        }
    }
}
```

---

## 🔄 IK 전환 예시

만약 애니메이션 중간에 왼손을 추가로 붙이는 연출을 하고 싶다면, `leftHandIKWeight`를 점진적으로 `1.0f`로 **Lerp**하면 자연스럽게 전환됩니다:

```csharp
// Update 함수에서 시간에 따라 IK 가중치를 변화시킬 수 있음
void Update()
{
    // 예: 왼손 IK 점점 증가
    if (Input.GetKey(KeyCode.Space))
    {
        leftHandIKWeight = Mathf.MoveTowards(leftHandIKWeight, 1.0f, Time.deltaTime * 2f);
    }
    else
    {
        leftHandIKWeight = Mathf.MoveTowards(leftHandIKWeight, 0.0f, Time.deltaTime * 2f);
    }
}
```

---

## ✅ 팁

* 카타나 프리팹은 항상 **정확한 위치에 IK 타겟이 있어야** 손이 어색하지 않음.
* **오른손은 항상 무기를 기준으로 회전 고정**, 왼손은 보조적으로 붙게 만드는 게 일반적.
* IK를 부드럽게 전환하고 싶다면 애니메이션 이벤트나 트랜지션 타이밍과 잘 맞춰야 합니다.

---

원한다면 카타나 모델에 맞는 `IK 타겟 배치 기준`도 이미지로 설명드릴 수 있어요! 😎

 *
 */