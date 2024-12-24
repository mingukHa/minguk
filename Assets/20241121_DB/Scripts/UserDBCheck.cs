using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class UserDBCheck : MonoBehaviour
{
    public event Action OnUserDataLoaded;  // 데이터 로드 완료 시 호출될 이벤트

    public class User_ItemData
    {
        public string id { get; set; }
        public int inventory_num { get; set; }
        public int item_num { get; set; }
        public string item_name { get; set; }
        public string item_state { get; set; }
    }
    [SerializeField]
    private GameObject user_itemList;  // user_item_list 오브젝트

    private List<User_ItemData> user_itemDatas;

    private void Start()
    {
        StartCoroutine(GetScoreCoroutine());
    }

    private IEnumerator GetScoreCoroutine()
    {
        string uri = "http://127.0.0.1/getuserinventory.php";

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(uri, string.Empty))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                string data = www.downloadHandler.text;

                //user_itemDatas = JsonConvert.DeserializeObject<List<User_ItemData>>(data);

                // 아이템 리스트의 하위 오브젝트와 데이터를 매칭
                Debug.Log("아이템 갯수:" + user_itemDatas.Count);
                for (int i = 0; i < user_itemDatas.Count; i++)
                {
                    if (i < user_itemList.transform.childCount)
                    {
                        // item_list 하위 오브젝트의 Item_hover 컴포넌트를 가져옴
                        User_Item_hover user_itemHover = user_itemList.transform.GetChild(i).GetComponent<User_Item_hover>();

                        if (user_itemHover != null)
                        {
                            // 아이템의 데이터(ItemData)를 해당 아이템 오브젝트에 저장
                            user_itemHover.SetItemData(user_itemDatas[i]);
                        }
                    }
                }

                //foreach (ItemData itemdata in itemDatas)
                //{
                //    Debug.Log(itemdata.id + ":" + itemdata.inventory_num + ":" + itemdata.item_num + ":" + itemdata.item_name + ":" + itemdata.item_state);
                //}
                foreach (User_ItemData itemdata in user_itemDatas)
                {
                    Debug.Log(itemdata.id + ":" + itemdata.inventory_num + ":" + itemdata.item_num + ":" + itemdata.item_name + ":" + itemdata.item_state);
                }
                OnUserDataLoaded?.Invoke();
            }
        }
    }
}