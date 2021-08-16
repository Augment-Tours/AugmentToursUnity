using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignupBehavior : MonoBehaviour
{
    public TMP_InputField emailTextBox;
    public TMP_InputField passwordTextBox;
    public TMP_InputField nameTextBox;

    public TMP_Text errorText;
    public Button signupButton;
    public Button signinButton;
    protected Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started Signup script");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        signupButton.onClick.AddListener(() => canSubmit());
        signinButton.onClick.AddListener(() => Login());
    }

    private void canSubmit()
    {
        // add some validation for password and confirm password here.
        CreateUserWithEmailAsync();
        StartCoroutine(addUser(nameTextBox.text, emailTextBox.text, passwordTextBox.text));
    }

    private void Login()
    {
      SceneManager.LoadScene("Signin");
    }

    private Task CreateUserWithEmailAsync()
    {
        string email = emailTextBox.text;
        string password = passwordTextBox.text;

        Debug.Log(String.Format("Attempting to create user {0}...", email));
        errorText.text = "Signing up...";
        // DisableUI();

        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
                    .ContinueWithOnMainThread((task) => {
                        // EnableUI();
                        bool complete = LogTaskCompletion(task, "User Creation");
                        if(complete) {
                          errorText.text = "Signed Up Successfully!";
                          SceneManager.LoadScene("MainScreen");
                        }
                        return task;
                    }).Unwrap();
    }

    IEnumerator addUser(string name, string email, string password )
    {
        Debug.Log("Register user " + name);
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("https://augment-tours-backend.herokuapp.com/accounts", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableUI()
  {
    emailTextBox.DeactivateInputField();
    passwordTextBox.DeactivateInputField();
    signupButton.interactable = false;
  }

  void EnableUI()
  {
    emailTextBox.ActivateInputField();
    passwordTextBox.ActivateInputField();
    signupButton.interactable = true;
  }

  // Log the result of the specified task, returning true if the task
  // completed successfully, false otherwise.
  protected bool LogTaskCompletion(Task task, string operation) {
    bool complete = false;
    if (task.IsCanceled) {
      Debug.Log(operation + " canceled.");
    } else if (task.IsFaulted) {
      Debug.Log(operation + " encountered an error.");
      foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
        string authErrorCode = "";
        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        if (firebaseEx != null) {
          authErrorCode = String.Format("AuthError.{0}: ",
            ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
          GetErrorMessage((Firebase.Auth.AuthError)firebaseEx.ErrorCode);
        }
        Debug.Log(authErrorCode + exception.ToString());
      }
    } else if (task.IsCompleted) {
      Debug.Log(operation + " completed");
      complete = true;
    }
    return complete;
  }


  private void GetErrorMessage(AuthError errorCode)
  {
    switch (errorCode)
    {
    //   case AuthError.MissingPassword:
    //     passwordErrorText.text = "Missing password.";
    //     passwordErrorText.enabled = true;
    //     break;
    //   case AuthError.WeakPassword:
    //     passwordErrorText.text = "Too weak of a password.";
    //     passwordErrorText.enabled = true;
    //     break;
    //   case AuthError.InvalidEmail:
    //     emailErrorText.text = "Invalid email.";
    //     emailErrorText.enabled = true;
    //     break;
    //   case AuthError.MissingEmail:
    //     emailErrorText.text = "Missing email.";
    //     emailErrorText.enabled = true;
    //     break;
    //   case AuthError.UserNotFound:
    //     emailErrorText.text = "Account not found.";
    //     emailErrorText.enabled = true;
    //     break;
    //   case AuthError.EmailAlreadyInUse:
    //     emailErrorText.text = "Email already in use.";
    //     emailErrorText.enabled = true;
    //     break;
      default:
        errorText.text = "Unknown error occurred. " + errorCode;
        errorText.enabled = true;
        break;
    }
  }
}
