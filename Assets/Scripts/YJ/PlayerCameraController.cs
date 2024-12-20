using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public Transform cameraRig; // ī�޶� ����
    public OVRInput.Controller leftController;  // ���� ��Ʈ�ѷ�
    public OVRInput.Controller rightController; // ������ ��Ʈ�ѷ�

    [SerializeField] private float moveSpeed = 1.0f;    // �̵� �ӵ�
    [SerializeField] private float accelerationFactor = 2.0f; // ���ӵ� ���
    [SerializeField] private float dampingFactor = 0.99f;     // ���� ���(����)

    private Vector3 velocity = Vector3.zero; // ���� �̵� �ӵ�
    private Quaternion initialLeftControllerRotation;  // ���� ��Ʈ�ѷ� �ʱ� ȸ��
    private Quaternion initialRightControllerRotation; // ������ ��Ʈ�ѷ� �ʱ� ȸ��
    private bool isLeftRotating = false; // ���� ȸ�� ����
    private bool isRightRotating = false; // ������ ȸ�� ����

    private void Update()
    {
        // �� ��Ʈ�ѷ��� �Է��� �����Ͽ� �̵� ó��
        bool isMoving = HandleMovement(leftController, rightController);

        if (!isMoving)
        {
            // ������ ��Ʈ�ѷ��� ȸ�� ó��
            HandleRotation(
                leftController, ref initialLeftControllerRotation, ref isLeftRotating, true); // ���� ��Ʈ�ѷ�: ��ȸ����
            HandleRotation(
                rightController, ref initialRightControllerRotation, ref isRightRotating, false); // ������ ��Ʈ�ѷ�: ��ȸ����
        }

        // ����(����) ó�� �Ǵ� �극��ũ ó��
        ApplyBrakingOrDamping(isMoving);

        // �̵� ����
        cameraRig.position += velocity * Time.deltaTime;
    }

    private bool HandleMovement(OVRInput.Controller left, OVRInput.Controller right)
    {
        // ���� ��Ʈ�ѷ��� �Է� ó��
        Vector3 leftControllerDirection = OVRInput.GetLocalControllerRotation(left) * Vector3.forward;
        Vector3 leftControllerMovement = Vector3.zero;
        if (Vector3.Dot(leftControllerDirection, Vector3.down) > 0.8f &&
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, left))
        {
            leftControllerMovement = OVRInput.GetLocalControllerVelocity(left);
        }

        // ������ ��Ʈ�ѷ��� �Է� ó��
        Vector3 rightControllerDirection = OVRInput.GetLocalControllerRotation(right) * Vector3.forward;
        Vector3 rightControllerMovement = Vector3.zero;
        if (Vector3.Dot(rightControllerDirection, Vector3.down) > 0.8f &&
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, right))
        {
            rightControllerMovement = OVRInput.GetLocalControllerVelocity(right);
        }

        // �� ��Ʈ�ѷ��� �Է��� ����
        Vector3 combinedMovement = leftControllerMovement + rightControllerMovement;
        float acceleration = combinedMovement.magnitude * accelerationFactor;

        if (combinedMovement.z < -0.1f)
        {
            velocity += cameraRig.forward * (-combinedMovement.z * moveSpeed * acceleration * Time.deltaTime);
            return true; // �̵� ��
        }
        else if (combinedMovement.z > 0.1f)
        {
            velocity -= cameraRig.forward * (combinedMovement.z * moveSpeed * acceleration * Time.deltaTime);
            return true; // �̵� ��
        }

        return false; // �̵� �� �ƴ�
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

            // Yaw (y�� ȸ��) ��ȭ ���
            float yawDelta = Mathf.DeltaAngle(0, rotationDelta.eulerAngles.y);

            // ���⿡ ���� ȸ�� ó��
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
            // �̵� ���� ��� �극��ũ ȿ���� �������� ����
            return;
        }

        bool isLeftTriggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController);
        bool isRightTriggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController);

        if ((isLeftTriggerPressed || isRightTriggerPressed) && velocity.magnitude < 0.5f)
        {
            // �ӵ��� 0.5f ������ �� �극��ũ ȿ�� ����
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.1f); // �ӵ��� õõ�� ���ҽ�Ŵ
        }
        else
        {
            // �� �̿��� ��� �ڿ������� ����(����) ����
            velocity *= dampingFactor;
        }
    }
}
