using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class AuthSetup : MonoBehaviour
{
    public AuthManager authManager;
    private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    // Start is called before the first frame update
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                authManager.InitializeFirebase();
            } else {
                Debug.LogError("Could not resolve all Firebase Dependencies: " + dependencyStatus);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
