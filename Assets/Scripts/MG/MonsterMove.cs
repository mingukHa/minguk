using UnityEngine;
using UnityEngine.AI;

public class Monsterai : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 10f; // 충돌 탐지 반경
    [SerializeField]
    private float viewAngle = 45f; // 시야각 (좌우 45도)
    [SerializeField]
    private LayerMask detectionLayer; // 감지 대상의 레이어
    [SerializeField]
    private float visionDistance = 5f; // 시야 범위

    private enum MosterState { Idle, Walking, Quest, Attack } //애니메이터 상태

    private void Update()
    {
        DetectTargetsInView();
    }

    private void DetectTargetsInView() //탐지 범위
    {
        // 탐지 반경 내의 모든 Collider 가져오기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        foreach (Collider collider in hitColliders)
        {
            Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;

            // 시야각 내에 있는지 확인
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if (angleToTarget > viewAngle / 2) continue;

            // 타겟이 시야 거리 내에 있는지 확인
            if (Vector3.Distance(transform.position, collider.transform.position) > visionDistance) continue;

            Debug.Log("시야 내 타겟 발견: " + collider.name);
        }
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






















