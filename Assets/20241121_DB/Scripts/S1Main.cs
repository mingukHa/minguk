using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class S1Main : MonoBehaviour
{
    [SerializeField]
    private Button Jclosebt = null;
    [SerializeField]
    private Button Loginbt = null;
    [SerializeField]
    private Button Eclosebt = null;
    [SerializeField]
    private Button joinbt = null;
    [SerializeField]
    private Button exitbt = null;
    [SerializeField]
    private GameObject joinUI = null;
    [SerializeField]
    private GameObject exitUI = null;
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;

    // Firebase 객체들
    private DatabaseReference database;

    private void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase 초기화 완료");
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {task.Result}");
            }
        });

        // 버튼 이벤트 연결
        Loginbt.onClick.AddListener(() => Login(username.text, password.text));
        joinbt.onClick.AddListener(() => OnjoinUI(true));
        exitbt.onClick.AddListener(() => OnExitUI(true));
        Jclosebt.onClick.AddListener(() => Jclose(false));
        Eclosebt.onClick.AddListener(() => Eclose(false));
    }

    private void Login(string id, string password)
    {
        Debug.Log($"로그인 시도: {id}, {password}");

        // Firebase Realtime Database에서 사용자 데이터 확인
        database.Child("users").OrderByChild("id").EqualTo(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"로그인 실패: {task.Exception}");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                foreach (var user in snapshot.Children)
                {
                    // Firebase Snapshot을 Dictionary<string, object>로 변환
                    var userData = user.Value as Dictionary<string, object>;

                    if (userData != null && userData.ContainsKey("password"))
                    {
                        string storedPassword = userData["password"].ToString();

                        if (storedPassword == password)
                        {
                            Debug.Log("로그인 성공");
                            SceneManager.LoadScene("Scene2"); // 다음 씬으로 전환
                            return;
                        }
                        else
                        {
                            Debug.LogError("비밀번호가 일치하지 않습니다.");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError("사용자 데이터에 비밀번호가 없습니다.");
                        return;
                    }
                }
            }
            else
            {
                Debug.LogError("사용자를 찾을 수 없습니다.");
            }
        });
    }

    private void Jclose(bool close)
    {
        joinUI.SetActive(close);
    }

    private void Eclose(bool close)
    {
        exitUI.SetActive(close);
    }

    private void OnjoinUI(bool join)
    {
        joinUI.SetActive(join);
    }

    private void OnExitUI(bool exit)
    {
        exitUI.SetActive(exit);
    }

    [System.Serializable]
    public class User
    {
        public string id;       // 사용자 ID
        public string password; // 패스워드

        public User(string id, string password)
        {
            this.id = id;
            this.password = password;
        }
    }
}





