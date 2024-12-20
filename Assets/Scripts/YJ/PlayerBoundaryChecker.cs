using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerBoundaryChecker : MonoBehaviour
{
    private List<Vector3> boundaryPoints = new List<Vector3>();
    private bool isOutsideBoundary = false;

    private void Update()
    {
        // HMD 위치 확인
        Vector3 headPosition = Camera.main.transform.position;

        // Boundary Points 업데이트
        UpdateBoundaryPoints();

        // 현재 위치가 Boundary 내인지 확인
        bool isInsideBoundary = IsWithinBoundary(headPosition);

        if (!isInsideBoundary && !isOutsideBoundary)
        {
            // 경계를 처음 벗어났을 때
            Debug.LogWarning("경계를 벗어났습니다!");
            isOutsideBoundary = true;
        }
        else if (isInsideBoundary && isOutsideBoundary)
        {
            // 경계로 다시 돌아왔을 때
            Debug.Log("경계 안으로 들어왔습니다!");
            isOutsideBoundary = false;
        }
    }

    private void UpdateBoundaryPoints()
    {
        // XRInputSubsystem을 통해 Boundary Points 가져오기
        var inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(inputSubsystems);

        foreach (var subsystem in inputSubsystems)
        {
            if (subsystem.TryGetBoundaryPoints(boundaryPoints) && boundaryPoints.Count > 0)
            {
                return; // 성공적으로 경계 점을 가져왔음
            }
        }

        Debug.LogWarning("경계점을 찾을 수 없습니다.");
    }

    private bool IsWithinBoundary(Vector3 position)
    {
        if (boundaryPoints == null || boundaryPoints.Count == 0)
        {
            return true; // 경계 정보가 없으면 기본적으로 경계 내부로 간주
        }

        // Boundary Points를 기준으로 내부/외부 확인
        var boundaryPlane = new Plane(Vector3.up, boundaryPoints[0]); // 평면 생성 (위치와 방향에 맞게 조정 가능)

        foreach (var point in boundaryPoints)
        {
            if (!boundaryPlane.GetSide(position))
            {
                return false; // 경계 외부
            }
        }

        return true; // 경계 내부
    }
}
