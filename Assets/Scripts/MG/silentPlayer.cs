using UnityEngine;

public class slientPlayer : MonoBehaviour
{
    public CharacterController characterController;
    public float moveSpeed = 6f; // 이동 속도
    public float rotateSpeed = 180f; // 회전 속도
    public float jumpHeight = 2f; // 점프 높이
    public float gravity = -9.81f; // 중력 값
    public GameObject gameObject1;
    private Vector3 velocity; // 점프와 중력 속도 관리
    private bool isGrounded; // 바닥에 있는지 여부

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            gameObject1.SetActive(true);
        // 바닥 감지
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 바닥에 있을 때 중력 초기화
        }

        // 이동 및 회전
        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");

        // 전진/후진 이동
        Vector3 move = transform.forward * axisV * moveSpeed;
        characterController.SimpleMove(move); // SimpleMove는 이동에만 사용 (중력은 별도로 처리)

        // 회전
        transform.Rotate(0, axisH * rotateSpeed * Time.deltaTime, 0);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // 점프 속도 계산
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;

        // 캐릭터 컨트롤러로 중력 속도 반영
        characterController.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌을 감지함");
    }


}

