using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class DBCheck : MonoBehaviour
{
    public class ItemData
    {
        public string id { get; set; }
        public int inventory_num { get; set; }
        public int item_num { get; set; }
        public string item_name { get; set; }
        public string item_state { get; set; }
    }
    [SerializeField]
    private GameObject itemList;  // item_list 오브젝트
    private List<ItemData> itemDatas;

    private void Start()
    {
        StartCoroutine(GetScoreCoroutine());
    }

    private IEnumerator GetScoreCoroutine()
    {
        string uri = "http://127.0.0.1/getitem.php";

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

                itemDatas = JsonConvert.DeserializeObject<List<ItemData>>(data);
                
                // 아이템 리스트의 하위 오브젝트와 데이터를 매칭
                Debug.Log("아이템 갯수:" + itemDatas.Count);
                for (int i = 0; i < itemDatas.Count; i++)
                {
                    if (i < itemList.transform.childCount)
                    {
                        // item_list 하위 오브젝트의 Item_hover 컴포넌트를 가져옴
                        Item_hover itemHover = itemList.transform.GetChild(i).GetComponent<Item_hover>();

                        if (itemHover != null)
                        {
                            // 아이템의 데이터(ItemData)를 해당 아이템 오브젝트에 저장
                            itemHover.SetItemData(itemDatas[i]);
                        }
                    }
                }

                //foreach (ItemData itemdata in itemDatas)
                //{
                //    Debug.Log(itemdata.id + ":" + itemdata.inventory_num + ":" + itemdata.item_num + ":" + itemdata.item_name + ":" + itemdata.item_state);
                //}
                foreach (ItemData itemdata in itemDatas)
                {
                    Debug.Log(itemdata.item_num + ":" + itemdata.item_name + ":" + itemdata.item_state);
                }
            }
        }
    }
}
