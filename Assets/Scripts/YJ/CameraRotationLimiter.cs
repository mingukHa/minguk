using UnityEngine;

public class CameraRotationLimiter : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // 카메라 트랜스폼
    [SerializeField] private float minYRotation = -90f; // 최소 Y축 회전 (왼쪽)
    [SerializeField] private float maxYRotation = 90f;  // 최대 Y축 회전 (오른쪽)

    private void Update()
    {
        // 카메라의 현재 로컬 회전을 가져옴
        Vector3 currentRotation = cameraTransform.localEulerAngles;

        // Y축 회전값을 제한 (0~360 범위를 -180~180으로 변환)
        float clampedYRotation = Mathf.Clamp(
            (currentRotation.y > 180) ? currentRotation.y - 360 : currentRotation.y,
            minYRotation, maxYRotation
        );

        // 제한된 값을 다시 로컬 회전에 적용
        cameraTransform.localEulerAngles = new Vector3(currentRotation.x, clampedYRotation, currentRotation.z);
    }
}
