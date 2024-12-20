using UnityEngine;

public class WheelChairSync : MonoBehaviour
{
    public Transform cameraRig; // Camera Rig의 Transform
    //public Vector3 offset = new Vector3(0f, 0f, 0f); // Camera Rig와 휠체어간 간격(플레이어 모델 적용하면 높이에 맞출 것)

    private Transform wheelchair;

    private void Start()
    {
        wheelchair = transform;
    }

    private void Update()
    {
        // 위치값 동기화 + 오프셋 적용
        //wheelchair.position = cameraRig.position + cameraRig.rotation * offset;

        // 위치값 동기화
        wheelchair.position = cameraRig.position;

        // 회전값 동기화
        wheelchair.rotation = cameraRig.rotation;
    }
}
