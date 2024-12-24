using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using System.Text.RegularExpressions;

public class Account : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField Username;
    [SerializeField]
    private TMP_InputField Password;
    [SerializeField]
    private TMP_InputField PasswordCheck;
    [SerializeField]
    private TextMeshProUGUI PasswordMessage;
    [SerializeField]
    private TextMeshProUGUI PasswordMatchMessage;
    [SerializeField]
    private TextMeshProUGUI PasswordCheckIcon;
    [SerializeField]
    private TextMeshProUGUI PasswordMatchIcon;
    [SerializeField]
    private Button SignupButton;
    [SerializeField]
    private GameObject AccountUI;

    // Firebase 객체
    private DatabaseReference database;

    private void Start()
    {
        SignupButton.interactable = false; // 회원가입 버튼 비활성화
        Password.onValueChanged.AddListener(OnPasswordChanged);
        PasswordCheck.onValueChanged.AddListener(OnPasswordCheckChanged);
        SignupButton.onClick.AddListener(() => RegisterUser(Username.text, Password.text));
    }

    private void Update()
    {
        EnableSignupButton();
    }

    private void EnableSignupButton()
    {
        if (PasswordCheckIcon.color == Color.green && PasswordMatchIcon.color == Color.green)
        {
            SignupButton.interactable = true; // 모든 조건이 충족되면 활성화
        }
        else
        {
            SignupButton.interactable = false; // 조건이 충족되지 않으면 비활성화
        }
    }

    private void RegisterUser(string username, string password)
    {
        Debug.Log($"{username}, {password} 값 들어옴");

        string userId = database.Push().Key; // 고유 키 생성
        User user = new User(username, password);

        // Firebase Realtime Database에 데이터 저장
        database.Child("users").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(user)).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"사용자 데이터 저장 실패: {task.Exception}");
            }
            else
            {
                Debug.Log("회원가입 성공");
                PasswordMessage.text = "회원가입이 완료되었습니다.";
                PasswordMessage.color = Color.green;
                AccountUI.SetActive(false); // 회원가입 UI 닫기
            }
        });
    }

    private void OnPasswordChanged(string password)
    {
        if (IsPasswordValid(password))
        {
            PasswordMessage.text = "사용 가능한 비밀번호입니다.";
            PasswordMessage.color = Color.green;
            PasswordCheckIcon.text = "O";
            PasswordCheckIcon.color = Color.green;
        }
        else
        {
            PasswordMessage.text = "비밀번호는 영어와 숫자로 6~10자 이내로 입력하세요.";
            PasswordMessage.color = Color.red;
            PasswordCheckIcon.text = "X";
            PasswordCheckIcon.color = Color.red;
        }
    }

    private void OnPasswordCheckChanged(string confirmPassword)
    {
        if (IsPasswordMatch())
        {
            PasswordMatchMessage.text = "비밀번호가 일치합니다.";
            PasswordMatchMessage.color = Color.green;
            PasswordMatchIcon.text = "O";
            PasswordMatchIcon.color = Color.green;
        }
        else
        {
            PasswordMatchMessage.text = "비밀번호가 일치하지 않습니다.";
            PasswordMatchMessage.color = Color.red;
            PasswordMatchIcon.text = "X";
            PasswordMatchIcon.color = Color.red;
        }
    }

    private bool IsPasswordValid(string password)
    {
        if (password.Length < 6 || password.Length > 10)
        {
            return false;
        }

        Regex regex = new Regex("^[a-zA-Z0-9]+$");
        return regex.IsMatch(password);
    }

    private bool IsPasswordMatch()
    {
        return Password.text == PasswordCheck.text;
    }
}

[System.Serializable]
public class User
{
    public string username;
    public string password;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}



