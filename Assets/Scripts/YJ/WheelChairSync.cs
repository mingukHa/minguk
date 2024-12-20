using UnityEngine;

public class WheelChairSync : MonoBehaviour
{
    public Transform cameraRig; // Camera Rig�� Transform
    //public Vector3 offset = new Vector3(0f, 0f, 0f); // Camera Rig�� ��ü� ����(�÷��̾� �� �����ϸ� ���̿� ���� ��)

    private Transform wheelchair;

    private void Start()
    {
        wheelchair = transform;
    }

    private void Update()
    {
        // ��ġ�� ����ȭ + ������ ����
        //wheelchair.position = cameraRig.position + cameraRig.rotation * offset;

        // ��ġ�� ����ȭ
        wheelchair.position = cameraRig.position;

        // ȸ���� ����ȭ
        wheelchair.rotation = cameraRig.rotation;
    }
}
