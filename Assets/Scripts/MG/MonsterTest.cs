using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class MonsterTest : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 10f; // 충격 탐지 반경
    [SerializeField]
    private float viewAngle = 60f; // 근접 시야각 (좌우 60도)
    [SerializeField]
    private LayerMask detectionLayer; // 감지 대상의 레이어
    [SerializeField]
    private float visionDistance = 5f; // 근접 시야 거리
    [SerializeField]
    private float idleTimeLimit = 10f; // 탐지 없을 때 원래 위치로 복귀 시간
    [SerializeField]
    private NavMeshAgent navAgent;
    private Animator animator;
    private enum MonsterState { Idle, Walking, Quest, Attack, Returning , Detect} //대기, 걷기 , 탐색, 공격, 돌아가기
    private MonsterState currentState = MonsterState.Idle;

    private Vector3 originalPosition; // 몬스터 원래 위치
    private Vector3 targetPosition; // 이동할 목표 지점
    private Transform detectedTarget; // 탐지된 타겟
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        originalPosition = transform.position; //원래 지점을 저장 해두고
    }

    private void Update()
    {
        // 타겟 감지는 모든 상태에서 지속적으로 실행
        DetectTargetsInView();

        switch (currentState)
        {
            case MonsterState.Idle: //기본 상태에서는
                
                break;
            case MonsterState.Walking: //걸을 때는
                
                break;
            case MonsterState.Quest: //탐색 할 때는
               
                break;
            case MonsterState.Attack: //공격시
                
                break;
            case MonsterState.Returning: //돌아간다
                
                break;
            case MonsterState.Detect: //근접 탐지 시

                break;
        }
    }

    private void DetectTargetsInView() //기본 탐지 범위
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        //구형 콜라이더를 그려서 탐색
        foreach (Collider collider in hitColliders) //콜라이더에 탐지 된 정보의 배열을 가져온다
        {
            Vector3 directionToTarget = (collider.transform.position - transform.position).normalized; //콜라이더의 포지션에 나의 포지션을 빼서 해당 방향을 구한다
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); 

            // 시야각 및 거리 확인
            if (angleToTarget <= viewAngle / 2 && Vector3.Distance(transform.position, collider.transform.position) <= visionDistance)
            {
                if (collider.CompareTag("Player")) //감지 된 물체가 플레이어 태그를 가지고 있다면
                {
                    Debug.Log("플레이어 발견");
                    detectedTarget = collider.transform; //타겟 플레이어의 콜라이더 트랜스폼을 가져온다
                    currentState = MonsterState.Attack; // 플레이어 발견 시 즉시 공격 상태로 전환
                    Debug.Log("플레이어 공격");
                    return;
                }
                else
                {
                    Debug.Log("플레이어 찾지 못 함");
                    targetPosition = collider.transform.position;
                    if (currentState != MonsterState.Walking)
                        currentState = MonsterState.Walking; // 충격 또는 일반 타겟 감지 시 Walking으로 전환                   
                    return;
                }
            }
        }

        // 타겟이 없으면 감지된 타겟 초기화
        detectedTarget = null;
    }

    

    private void OnDrawGizmosSelected()
    {
        // 탐지 반경 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 시야각 시각화
        Vector3 forward = transform.forward * visionDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}

