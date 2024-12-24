using TMPro;
using UnityEngine;

public class status_text : MonoBehaviour
{
    [SerializeField]
    private TMP_Text shop_item_name;  // 텍스트를 출력할 TMP_Text 컴포넌트
    [SerializeField]
    private TMP_Text shop_item_status;  // 텍스트를 출력할 TMP_Text 컴포넌트

    public void UpdateText(string item_name_Text, string item_status_Text)
    {
        if (shop_item_name != null && shop_item_status != null)
        {
            shop_item_name.text = item_name_Text;  // 전달받은 텍스트를 TMP_Text에 출력
            shop_item_status.text = item_status_Text;  // 전달받은 텍스트를 TMP_Text에 출력
        }
        else
        {
            Debug.LogError("TMP_Text 컴포넌트가 할당되지 않았습니다.");
        }
    }
}