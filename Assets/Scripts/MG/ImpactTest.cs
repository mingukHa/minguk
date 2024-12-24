using UnityEngine;

public class ImpactTest : MonoBehaviour
{
    public GameObject targetObject; // 충돌 테스트용 대상 물체

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            // 화면에서 클릭한 위치를 Ray로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 위치로 물체 이동
                Vector3 clickPosition = hit.point;
                targetObject.transform.position = clickPosition + Vector3.up;

                Debug.Log("좌표 이동: " + clickPosition);

                // 충돌 확인
                Collider[] hitColliders = Physics.OverlapSphere(clickPosition, 0.1f);
                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject != targetObject)
                    {
                        Debug.Log("충돌: " + collider.name);
                    }
                }
            }
            
        }
        
        
    }
}

