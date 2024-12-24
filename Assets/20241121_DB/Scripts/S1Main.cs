using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;

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

    private FirebaseAuth auth;

    private void Start()
    {
        // Firebase Authentication 초기화
        auth = FirebaseAuth.DefaultInstance;

        // 버튼 이벤트 연결
        Loginbt.onClick.AddListener(() => Login(username.text, password.text));
        joinbt.onClick.AddListener(() => OnjoinUI(true));
        exitbt.onClick.AddListener(() => OnExitUI(true));
        Jclosebt.onClick.AddListener(() => Jclose(false));
        Eclosebt.onClick.AddListener(() => Eclose(false));
    }
    
    // Firebase로 로그인
    private void Login(string email, string password)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"로그인 실패: {task.Exception}");
                return;
            }

            // AuthResult에서 FirebaseUser 가져오기
            Firebase.Auth.AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;

            Debug.Log($"로그인 성공! 사용자 ID: {user.UserId}, 이메일: {user.Email}");
            SceneManager.LoadScene("Scene2"); // 로그인 성공 시 다음 씬으로 전환
        });
    }


    // 닫기 버튼 처리
    private void Jclose(bool close)
    {
        joinUI.SetActive(close);
    }

    private void Eclose(bool close)
    {
        exitUI.SetActive(close);
    }

    private void OnExitUI(bool exit)
    {
        exitUI.SetActive(exit);
    }

    private void OnjoinUI(bool join)
    {
        joinUI.SetActive(join);
    }
}


