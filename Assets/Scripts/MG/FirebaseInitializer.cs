using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseAuth Auth { get; private set; } // Firebase Authentication
    public static DatabaseReference Database { get; private set; } // Firebase Realtime Database

    private void Awake()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Firebase 사용 준비 완료
                Debug.Log("Firebase 초기화 성공!");

                // Firebase 서비스 인스턴스 생성
                Auth = FirebaseAuth.DefaultInstance;
                Database = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                // Firebase 초기화 실패
                Debug.LogError($"Firebase 초기화 실패: {task.Result}");
            }
        });
    }
}

