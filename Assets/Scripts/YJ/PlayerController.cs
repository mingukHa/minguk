using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public Transform cameraRig; // 카메라 리그
    public OVRInput.Controller leftController;  // 왼쪽 컨트롤러
    public OVRInput.Controller rightController; // 오른쪽 컨트롤러

    [SerializeField] private float moveSpeed = 1.0f;    // 이동 속도
    [SerializeField] private float accelerationFactor = 2.0f; // 가속도 계수
    [SerializeField] private float dampingFactor = 0.99f;     // 감속 계수(관성)

    private Vector3 velocity = Vector3.zero; // 현재 이동 속도
    private Quaternion initialLeftControllerRotation;  // 왼쪽 컨트롤러 초기 회전
    private Quaternion initialRightControllerRotation; // 오른쪽 컨트롤러 초기 회전
    private bool isLeftRotating = false; // 왼쪽 회전 상태
    private bool isRightRotating = false; // 오른쪽 회전 상태

    private void Update()
    {
        // 두 컨트롤러의 입력을 결합하여 이동 처리
        bool isMoving = HandleMovement(leftController, rightController);

        if (!isMoving)
        {
            // 각각의 컨트롤러로 회전 처리
            HandleRotation(
                leftController, ref initialLeftControllerRotation, ref isLeftRotating, true); // 왼쪽 컨트롤러: 우회전만
            HandleRotation(
                rightController, ref initialRightControllerRotation, ref isRightRotating, false); // 오른쪽 컨트롤러: 좌회전만
        }

        // 감속(관성) 처리 또는 브레이크 처리
        ApplyBrakingOrDamping(isMoving);

        // 이동 적용
        cameraRig.position += velocity * Time.deltaTime;
    }

    private bool HandleMovement(OVRInput.Controller left, OVRInput.Controller right)
    {
        // 왼쪽 컨트롤러의 입력 처리
        Vector3 leftControllerDirection = OVRInput.GetLocalControllerRotation(left) * Vector3.forward;
        Vector3 leftControllerMovement = Vector3.zero;
        if (Vector3.Dot(leftControllerDirection, Vector3.down) > 0.8f &&
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, left))
        {
            leftControllerMovement = OVRInput.GetLocalControllerVelocity(left);
        }

        // 오른쪽 컨트롤러의 입력 처리
        Vector3 rightControllerDirection = OVRInput.GetLocalControllerRotation(right) * Vector3.forward;
        Vector3 rightControllerMovement = Vector3.zero;
        if (Vector3.Dot(rightControllerDirection, Vector3.down) > 0.8f &&
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, right))
        {
            rightControllerMovement = OVRInput.GetLocalControllerVelocity(right);
        }

        // 두 컨트롤러의 입력을 결합
        Vector3 combinedMovement = leftControllerMovement + rightControllerMovement;
        float acceleration = combinedMovement.magnitude * accelerationFactor;

        if (combinedMovement.z < -0.1f)
        {
            velocity += cameraRig.forward * (-combinedMovement.z * moveSpeed * acceleration * Time.deltaTime);
            return true; // 이동 중
        }
        else if (combinedMovement.z > 0.1f)
        {
            velocity -= cameraRig.forward * (combinedMovement.z * moveSpeed * acceleration * Time.deltaTime);
            return true; // 이동 중
        }

        return false; // 이동 중 아님
    }

    private void HandleRotation(OVRInput.Controller controller, ref Quaternion initialRotation, ref bool isRotating, bool isRightDirectionOnly)
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            initialRotation = OVRInput.GetLocalControllerRotation(controller);
            isRotating = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            Quaternion currentRotation = OVRInput.GetLocalControllerRotation(controller);
            Quaternion rotationDelta = currentRotation * Quaternion.Inverse(initialRotation);

            // Yaw (y축 회전) 변화 계산
            float yawDelta = Mathf.DeltaAngle(0, rotationDelta.eulerAngles.y);

            // 방향에 따라 회전 처리
            if ((isRightDirectionOnly && yawDelta > 0) || (!isRightDirectionOnly && yawDelta < 0))
            {
                cameraRig.Rotate(Vector3.up, yawDelta);
            }

            initialRotation = currentRotation;
        }
    }

    private void ApplyBrakingOrDamping(bool isMoving)
    {
        if (isMoving)
        {
            // 이동 중인 경우 브레이크 효과를 적용하지 않음
            return;
        }

        bool isLeftTriggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController);
        bool isRightTriggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController);

        if ((isLeftTriggerPressed || isRightTriggerPressed) && velocity.magnitude < 0.5f)
        {
            // 속도가 0.5f 이하일 때 브레이크 효과 적용
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.1f); // 속도를 천천히 감소시킴
        }
        else
        {
            // 그 이외의 경우 자연스러운 감속(관성) 적용
            velocity *= dampingFactor;
        }
    }
}
