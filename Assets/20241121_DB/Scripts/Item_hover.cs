using UnityEngine;
using static DBCheck;

public class Item_hover : MonoBehaviour
{
    // 활성화할 특정 게임오브젝트
    [SerializeField]
    private GameObject statusBox;
    [SerializeField]
    private status_text statusText;  // 아이템에 맞는 텍스트를 업데이트할 statusText

    private ItemData itemData;  // 해당 아이템의 데이터 저장

    void Start()
    {
        // statusBox를 비활성화 상태로 시작
        if (statusBox != null)
        {
            statusBox.SetActive(false);
        }

        // statusBox에서 status_text 컴포넌트를 찾음
        //statusText = statusBox.GetComponent<status_text>();
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;  // 아이템 데이터를 저장
    }

    void OnMouseEnter()
    {
        // 마우스 커서가 아이템 오브젝트에 올라갔을 때
        if (statusBox != null)
        {
            // statusBox를 활성화
            statusBox.SetActive(true);

            // 아이템 오브젝트의 중심점 계산
            Renderer itemRenderer = GetComponent<Renderer>();
            Vector3 itemCenter = itemRenderer.bounds.center; // 아이템 오브젝트의 중심점

            // statusBox의 크기 계산 (하위 오브젝트의 Renderer 사용)
            Renderer statusChildRenderer = statusBox.GetComponentInChildren<Renderer>();
            Vector3 statusBoxSize = statusChildRenderer.bounds.size;

            // 아이템 오브젝트의 중심점에서 statusBox의 좌상단이 오도록 위치 조정
            Vector3 newPosition = itemCenter + new Vector3(statusBoxSize.x / 2, 0.5f, -statusBoxSize.z / 2);

            // statusBox의 위치를 새로 계산된 위치로 설정
            statusBox.transform.position = newPosition;
        }

        // 아이템에 해당하는 이름을 statusText에 전달
        if (itemData != null)
        {
            statusText.UpdateText(itemData.item_name, itemData.item_state);  // 아이템 데이터의 이름을 텍스트로 업데이트
        }
    }

    void OnMouseExit()
    {
        // 마우스 커서가 아이템 오브젝트에서 벗어났을 때
        if (statusBox != null)
        {
            // statusBox를 비활성화
            statusBox.SetActive(false);
        }
    }
}
