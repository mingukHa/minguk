using UnityEngine;
using static UserDBCheck;
using static DBCheck;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public DBCheck dbCheck;
    public UserDBCheck userdbCheck;

    private GameObject[,] itemList = new GameObject[4, 1]; // 인벤토리 아이템을 담을 1 x 4 배열
    private GameObject go;                                 // Ray와 상호작용할 상점 아이템

    //private User_ItemData useritem; //유저인벤 아이템 DB데이터
    [SerializeField]
    private GameObject storeitem_list; //상점 아이템 리스트
    [SerializeField]
    private GameObject invenItemsHolder; // 인벤토리에 생성될 아이템 홀더, 유저 인벤 아이템 리스트
    [SerializeField]
    private Camera subCamera;            // 상점 오브젝트를 비출 서브카메라

    private float invenItemScale;        // 인벤토리에 들어간 아이템 스케일링 배수
    private Vector3 invenItemDist;       // 인벤토리 아이템간 간격
    private Vector3 startPos;            // 인벤토리 아이템 배치 시작 위치

    private Vector3 cursorPos;           // 마우스 커서 위치

    private List<Item_hover> itemHovers = new List<Item_hover>();// Item_hover(상점 아이템 DB저장됨) 컴포넌트들을 저장할 리스트
    private List<User_Item_hover> useritemHovers = new List<User_Item_hover>();
    private List<User_ItemData> userItemDataList = new List<User_ItemData>(); // 유저 아이템 데이터

    [SerializeField]
    private GameObject user_statusBox;  // 상점 아이템 상태 박스 (shop_item_status 오브젝트)
    [SerializeField]
    private status_text user_statusText; // 인벤 아이템 상태 텍스트 (InvenItemsHolder 오브젝트)

    //public void SetItemData(User_ItemData data)
    //{
    //    useritem = data;  // 아이템 데이터를 저장
    //}

    private void Start()
    {
        if (dbCheck == null)
        {
            Debug.LogError("dbCheck is not assigned in GameManager");
            return;
        }

        // DBCheck에서 데이터 로드 완료 후 작업을 수행하도록 이벤트 등록
        //dbCheck.OnDataLoaded += OnDataLoaded;
        userdbCheck.OnUserDataLoaded += OnUserDataLoaded;

        // 변수 초기화
        startPos = new Vector3(7f, 0.1f, 1.8f);
        invenItemDist = new Vector3(0f, 0f, -2.2f);
        invenItemScale = 2.5f;


        //// 아이템 리스트에 있는 모든 아이템 오브젝트에서 Item_hover 컴포넌트를 가져옴
        //foreach (Transform child in storeitem_list.transform)
        //{
        //    Item_hover itemHover = child.GetComponent<Item_hover>();
        //    if (itemHover != null)
        //    {
        //        Debug.Log("Item_hover component found for: " + child.name); // 아이템이 제대로 가져왔을 때 출력
        //    }
        //    else
        //    {
        //        Debug.Log("No Item_hover component on: " + child.name); // Item_hover 컴포넌트가 없을 때 출력
        //    }
        //}
        //// 아이템들의 데이터를 활용하는 예시
        //PrintItemData();
    }
    private void OnDataLoaded()
    {
        // DB에서 데이터가 로드된 후 수행할 작업
        Debug.Log("데이터 로드 완료");

        // 아이템 리스트에서 Item_hover 컴포넌트를 가져와 `itemHovers` 리스트에 추가
        itemHovers.Clear();  // 이전에 저장된 값이 있다면 초기화

        foreach (Transform child in storeitem_list.transform)
        {
            Item_hover itemHover = child.GetComponent<Item_hover>();
            if (itemHover != null)
            {
                itemHovers.Add(itemHover);  // 리스트에 추가
            }
            else
            {
                Debug.Log("No Item_hover component on: " + child.name);  // 컴포넌트가 없을 경우 로그 출력
            }
        }
        //PrintItemData();
    }
    //private void PrintItemData()
    //{
    //    foreach (Item_hover itemHover in itemHovers)
    //    {
    //        Debug.Log("gameManager Item");
    //        StoreItemData itemData = itemHover.GetItemData(); // Item_hover에서 저장된 데이터 가져오기

    //        // itemData의 각 속성 활용
    //        Debug.Log("gameManager Item Num: " + itemData.item_num);
    //        Debug.Log("gameManager Item Name: " + itemData.item_name);
    //        Debug.Log("gameManager Item State: " + itemData.item_state);
    //    }
    //}
    private void OnUserDataLoaded()
    {
        // 유저 인벤토리 데이터가 로드된 후 수행할 작업
        Debug.Log("유저 인벤 데이터 로드 완료");

        userItemDataList.Clear();  // 이전에 저장된 데이터가 있다면 초기화
        //userItemDataList.AddRange(userdbCheck.GetUserItemDatas()); // 유저 아이템 데이터 리스트 가져오기

        // 유저 인벤 아이템과 상점 아이템을 비교하고 인벤토리 복제
        CompareAndAddItemsToInventory();

        // 복제 후, Item_hover 제거 작업
        RemoveItemHoverFromInventory();
    }

    private void RemoveItemHoverFromInventory()
    {
        // 아이템 리스트를 순회하며 복제된 아이템들의 Item_hover 컴포넌트를 제거
        foreach (var item in itemList)
        {
            if (item != null)
            {
                Item_hover itemHover = item.GetComponent<Item_hover>();
                if (itemHover != null)
                {
                    Destroy(itemHover);  // Item_hover 컴포넌트 제거
                }

                // 그 후에 User_Item_hover를 추가
                User_Item_hover userItemHover = item.AddComponent<User_Item_hover>();
                if (userItemHover != null)
                {
                    // statusBox와 statusText를 해당 아이템에 할당
                   // userItemHover.statusBox = user_statusBox;
                   // userItemHover.statusText = user_statusText;
                }
            }
        }
    }

    private void CompareAndAddItemsToInventory()
    {
        foreach (var userItem in userItemDataList)
        {
            foreach (var storeItem in itemHovers)
            {
                // 유저 아이템 번호와 상점 아이템 번호 비교
                //if (userItem.item_num == storeItem.GetItemData().item_num)
                {
                    // 인벤토리에 이미 있는지 확인 후, 없으면 복제
                    AddItemToInventory(storeItem.gameObject);
                    break;
                }
            }
        }
    }
    private void AddItemToInventory(GameObject storeItem)
    {
        int itemCount = 0;

        // 인벤토리에 복사된 아이템이 배치될 위치 계산
        for (int row = 0; row < 4; ++row)
        {
            for (int col = 0; col < 1; ++col)
            {
                if (itemList[row, col] == null)
                {
                    Vector3 goPos = startPos + new Vector3(col * invenItemDist.x, 0, row * invenItemDist.z);

                    // 아이템 복제 및 위치 설정
                    GameObject newGo = Instantiate(
                        storeItem, goPos, Quaternion.Euler(
                            storeItem.transform.rotation.eulerAngles.x,
                            storeItem.transform.rotation.eulerAngles.y + 30f,
                            storeItem.transform.rotation.eulerAngles.z
                        ));

                    // 아이템 크기 조정
                    newGo.transform.localScale = storeItem.transform.localScale * invenItemScale;

                    // 인벤토리의 자식으로 설정
                    newGo.transform.SetParent(invenItemsHolder.transform);
                    itemList[row, col] = newGo;
                    newGo.tag = "InventoryItem";

                    itemCount++;
                    return;
                }
            }
        }
    }




    private void Update()
    {
        BuildObject();
    }

    // 아이템 생성, 파괴 메소드
    private void BuildObject()
    {
        cursorPos = Input.mousePosition;
        Ray ray = subCamera.ScreenPointToRay(cursorPos);
        RaycastHit hit;

        int itemCount = 0;

        // 마우스 왼쪽 버튼 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 커서 위치로 Ray 발사하여 닿는 오브젝트의 정보 반환
            Physics.Raycast(ray, out hit);

            // Ray에 맞은 콜라이더가 없을 때 null 예외처리
            if (hit.collider == null)
            {
                return;
            }

            // Ray가 콜라이더에 맞았을 때
            go = hit.collider.gameObject;

            // 스토어 아이템 태그면 인벤토리 위치로 복사
            if (go.CompareTag("StoreItem") && itemCount < 4)
            {
                // 인벤토리는 1 x 4 배치
                for (int row = 0; row < 4; ++row)
                {
                    for (int col = 0; col < 1; ++col)
                    {
                        if (itemList[row, col] == null)
                        {
                            // 인벤토리에 복사된 아이템이 배치될 위치
                            Vector3 goPos = startPos + new Vector3(col * invenItemDist.x, 0, row * invenItemDist.z);

                            // 해당 위치부터 아이템 생성 및 배치 후 오브젝트 방향 조정
                            GameObject newGo = Instantiate(
                                go, goPos, Quaternion.Euler(
                                    go.transform.rotation.eulerAngles.x,
                                    go.transform.rotation.eulerAngles.y + 30f,
                                    go.transform.rotation.eulerAngles.z
                                ));

                            // 스케일 확장
                            newGo.transform.localScale = go.transform.localScale * invenItemScale;

                            // 생성된 아이템 InvenItemsHolder로 배치 후 태그 변경
                            newGo.transform.SetParent(invenItemsHolder.transform);
                            itemList[row, col] = newGo;
                            newGo.tag = "InventoryItem";

                            ++itemCount;

                            return;
                        }
                    }
                }
            }
            // 인벤토리 아이템 태그면 인벤토리와 리스트에서 제거
            else if (go.CompareTag("InventoryItem"))
            {
                for (int row = 0; row < 4; ++row)
                {
                    for (int col = 0; col < 1; ++col)
                    {
                        if (itemList[row, col] == go)
                        {
                            itemList[row, col] = null;
                            Destroy(go);

                            --itemCount;

                            return;
                        }
                    }
                }
            }
        }
    }
}