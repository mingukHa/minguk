using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using System.Text.RegularExpressions;

public class LoginFireBase : MonoBehaviour
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
            passwordMessage.text = "비밀번호는 영어, 숫자로 6~10자 이내로 입력하세요.";
            passwordMessage.color = Color.red;
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync($"{username}", password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입이 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"회원가입 중 오류 발생: {task.Exception}");
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                // UID를 직접 추출하여 사용
                string userId = auth.CurrentUser?.UserId; // 현재 로그인된 사용자의 UID
                if (!string.IsNullOrEmpty(userId))
                {
                    Debug.Log($"회원가입 성공! 사용자 ID: {userId}");

                    // Firebase Realtime Database에 유저 정보 저장
                    database.Child("users").Child(username).SetValueAsync(userId).ContinueWith(dbTask =>
                    {
                        if (dbTask.IsCompletedSuccessfully)
                        {
                            Debug.Log("Firebase에 사용자 정보 저장 성공!");
                            acountUI.SetActive(false); // UI 닫기
                        }
                        else
                        {
                            Debug.LogError("Firebase에 사용자 정보 저장 실패: " + dbTask.Exception);
                        }
                    });
                }
                else
                {
                    Debug.LogError("회원가입 성공했지만 사용자 ID를 가져올 수 없습니다.");
                }
            }
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
