using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;

public class ProfileIcon : MonoBehaviour
{
    public TMP_Text userLetter;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        // errorText.text = user.Email;
        userLetter.text = user.Email[0].ToString().ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
