using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using System.Text.RegularExpressions;
using Firebase.Extensions;

public class FirebaseAccount : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField Username;
    [SerializeField]
    private TMP_InputField Password;
    [SerializeField]
    private TMP_InputField passwordcheck;
    [SerializeField]
    private TextMeshProUGUI passwordMessage;
    [SerializeField]
    private TextMeshProUGUI passwordMatchMessage;
    [SerializeField]
    private TextMeshProUGUI passwordCheckicon;
    [SerializeField]
    private TextMeshProUGUI passwordMatcgicon;
    [SerializeField]
    private TextMeshProUGUI idCheckMessage;
    [SerializeField]
    private TextMeshProUGUI idCheckIcon;
    [SerializeField]
    private Button idck;
    [SerializeField]
    private TextMeshProUGUI idckOX;
    [SerializeField]
    private TextMeshProUGUI idckMessage;
    [SerializeField]
    private Button acountet;
    [SerializeField]
    private GameObject acountUI;

    private FirebaseAuth auth;
    private DatabaseReference database;

    private void Start()
    {
        // Firebase 초기화
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance.RootReference;

        acountet.interactable = false; // 회원가입 버튼 비활성화
        Password.onValueChanged.AddListener(OnPasswordChanged);
        passwordcheck.onValueChanged.AddListener(OnPasswordCheckChanged);
        idck.onClick.AddListener(OnIdCheckClicked);
        acountet.onClick.AddListener(OnSignUpClicked);
    }

    private void FixedUpdate()
    {
        ToggleSignUpButton();
    }

    private void ToggleSignUpButton()
    {
        if (idckOX.color == Color.green && passwordCheckicon.color == Color.green && passwordMatcgicon.color == Color.green)
        {
            acountet.interactable = true; // 모든 조건이 초록색일 때 회원가입 버튼 활성화
        }
        else
        {
            acountet.interactable = false; // 조건이 하나라도 만족되지 않으면 비활성화
        }
    }

    // Firebase Realtime Database를 사용한 아이디 중복 확인
    private void OnIdCheckClicked()
    {
        string username = Username.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            idCheckMessage.text = "아이디를 입력하세요.";
            idCheckMessage.color = Color.red;
            idCheckIcon.text = "X";
            idCheckIcon.color = Color.red;
            return;
        }

        // Firebase에서 아이디 중복 확인
        database.Child("users").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    idckOX.color = Color.red;
                    idckOX.text = "X";
                    idckMessage.text = "이미 사용 중인 아이디입니다.";
                    idckMessage.color = Color.red;
                }
                else
                {
                    idckOX.color = Color.green;
                    idckOX.text = "O";
                    idckMessage.text = "사용 가능한 아이디입니다!";
                    idckMessage.color = Color.green;
                }
            }
            else
            {
                Debug.LogError("Firebase에서 데이터 가져오기 실패: " + task.Exception);
            }
        });
    }

    // Firebase Authentication을 사용한 회원가입
    private void OnSignUpClicked()
    {
        string username = Username.text.Trim();
        string password = Password.text.Trim();

        if (!IsPasswordMatch())
        {
            passwordMatchMessage.text = "비밀번호가 일치하지 않습니다.";
            passwordMatchMessage.color = Color.red;
            return;
        }

        if (!IsPassword(password))
        {
            passwordMessage.text = "비밀번호는 영어, 숫자로 4~10자 이내로 입력하세요.";
            passwordMessage.color = Color.red;
            return;
        }

        // Firebase Authentication으로 회원가입 처리
        auth.CreateUserWithEmailAndPasswordAsync($"{username}", password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"회원가입 실패: {task.Exception}");
                return;
            }

            // Firebase AuthResult를 통해 사용자 정보 가져오기
            Firebase.Auth.AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;

            Debug.Log($"회원가입 성공! 사용자 ID: {newUser.UserId}, 이메일: {newUser.Email}");

            // 사용자 정보를 Firebase Realtime Database에 저장
            database.Child("users").Child(username).SetValueAsync(newUser.UserId).ContinueWithOnMainThread(dbTask =>
            {
                if (dbTask.IsCompleted)
                {
                    Debug.Log("Firebase에 사용자 정보 저장 성공!");
                    acountUI.SetActive(false); // UI 닫기
                }
                else
                {
                    Debug.LogError("Firebase에 사용자 정보 저장 실패: " + dbTask.Exception);
                }
            });
        });
    }


    // 비밀번호 유효성 검사
    private void OnPasswordChanged(string password)
    {
        if (IsPassword(password))
        {
            passwordMessage.text = "사용 가능한 비밀번호입니다.";
            passwordMessage.color = Color.green;
            passwordCheckicon.text = "O";
            passwordCheckicon.color = Color.green;
        }
        else
        {
            passwordMessage.text = "비밀번호는 영어와 숫자로 4~10자 이내로 입력하세요.";
            passwordMessage.color = Color.red;
            passwordCheckicon.text = "X";
            passwordCheckicon.color = Color.red;
        }
    }

    // 비밀번호 일치 확인
    private void OnPasswordCheckChanged(string confirmPassword)
    {
        if (IsPasswordMatch())
        {
            passwordMatchMessage.text = "비밀번호가 일치합니다.";
            passwordMatchMessage.color = Color.green;
            passwordMatcgicon.text = "O";
            passwordMatcgicon.color = Color.green;
        }
        else
        {
            passwordMatchMessage.text = "비밀번호가 일치하지 않습니다.";
            passwordMatchMessage.color = Color.red;
            passwordMatcgicon.text = "X";
            passwordMatcgicon.color = Color.red;
        }
    }

    private bool IsPassword(string password)
    {
        if (password.Length < 4 || password.Length > 10)
        {
            return false;
        }
        Regex regex = new Regex("^[a-zA-Z0-9]+$");
        return regex.IsMatch(password);
    }

    private bool IsPasswordMatch()
    {
        return Password.text == passwordcheck.text;
    }
}


