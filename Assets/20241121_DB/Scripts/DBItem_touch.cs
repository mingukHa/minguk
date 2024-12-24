using System.Collections.Generic;
using UnityEngine;

public class DBItem_touch : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gameObjects;  // 클릭 가능한 게임 오브젝트 리스트

    private void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시, Raycast를 사용하여 클릭된 오브젝트 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 마우스 위치에서 Ray 생성
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Raycast로 맞은 오브젝트가 gameObjects 리스트에 포함되고, "shop_item" 태그를 가진 경우
                if (gameObjects.Contains(hit.transform.gameObject) && hit.transform.CompareTag("shop_item"))
                {
                    // 클릭된 게임 오브젝트의 이름을 콘솔에 출력
                    Debug.Log("Clicked on: " + hit.transform.gameObject.name);

                    // 해당 오브젝트의 인덱스를 구해서 출력
                    int index = gameObjects.IndexOf(hit.transform.gameObject);
                    Debug.Log("This is element number: " + index);
                }
            }
        }
    }
}