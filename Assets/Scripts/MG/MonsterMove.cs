using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 20f; // 탐지 반지름
    [SerializeField]
    private float stopDistance = 1f; // 목표 위치에 도달했는지 확인하는 거리

    public LayerMask detectionLayer; // 탐지할 레이어 (충돌 감지 대상)

    private NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트
    private Vector3 targetPosition; // 목표 위치

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // 목표에 도달했는지 확인
        if (navMeshAgent.remainingDistance <= stopDistance && !navMeshAgent.pathPending)
        {
            navMeshAgent.ResetPath(); // 경로 초기화
            Debug.Log("목표에 도달했습니다. 새로운 목표를 탐지합니다.");
        }
    }

    private void DetectTarget()
    {
        // 탐지 반지름 내 충돌 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        if (hitColliders.Length > 0)
        {
            // 충돌 지점 중 첫 번째를 목표 위치로 설정
            targetPosition = hitColliders[0].ClosestPoint(transform.position);

            Debug.Log("탐지된 목표 좌표: " + targetPosition);

            // NavMeshAgent를 사용해 목표로 이동
            navMeshAgent.SetDestination(targetPosition);
        }
        else
        {
            Debug.Log("탐지할 대상이 없습니다. 대기 중...");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 탐지 반지름 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}










