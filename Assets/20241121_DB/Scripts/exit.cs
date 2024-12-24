using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;

public class Exit : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField Username;
    [SerializeField]
    private TMP_InputField Password;
    [SerializeField]
    private Button exitbt;
    [SerializeField]
    private GameObject exitUI;


    // Firebase Realtime Database 참조
    private DatabaseReference database;

    private void Start()
    {
        // 버튼 클릭 시 계정 삭제 코루틴 실행
        exitbt.onClick.AddListener(() => DeleteAccount(Username.text.Trim(), Password.text.Trim()));
    }

    private void DeleteAccount(string username, string password)
    {
        Debug.Log($"계정 삭제 시도: {username}, {password}");

        // Firebase Realtime Database에서 사용자 데이터 검색
        database.Child("users").OrderByChild("username").EqualTo(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"계정 삭제 실패: {task.Exception}");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                foreach (var user in snapshot.Children)
                {
                    var userData = user.Value as System.Collections.Generic.Dictionary<string, object>;

                    if (userData != null && userData.ContainsKey("password"))
                    {
                        string storedPassword = userData["password"].ToString();

                        if (storedPassword == password)
                        {
                            // 사용자 데이터 삭제
                            database.Child("users").Child(user.Key).RemoveValueAsync().ContinueWithOnMainThread(deleteTask =>
                            {
                                if (deleteTask.IsFaulted || deleteTask.IsCanceled)
                                {
                                    Debug.LogError($"계정 삭제 중 오류 발생: {deleteTask.Exception}");
                                }
                                else
                                {
                                    Debug.Log("계정 삭제 완료");
                                    exitUI.SetActive(false); // UI 닫기
                                }
                            });
                            return;
                        }
                        else
                        {
                            Debug.LogError("비밀번호가 일치하지 않습니다.");
                            return;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("해당 아이디를 찾을 수 없습니다.");
            }
        });
    }
}
